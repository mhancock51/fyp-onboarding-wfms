using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowTemplateRepository : IRepository<WorkflowTemplateTable>
    {
        public Task<WorkflowTemplateTable?> GetWorkflowTemplateByName(string name);
        public Task<ServerResponse<string, string>> InsertWorkflowTemplate(WorkflowTemplateTable workflowTemplate, List<WorkflowTemplateNodeTable> nodes, List<NodeTaskDependencyTable> dependencies);        
    }

    public class WorkflowTemplateRepository : BaseRepository<WorkflowTemplateTable>, IWorkflowTemplateRepository
    {
        private readonly ILogger<WorkflowTemplateRepository> _logger;

        public WorkflowTemplateRepository(ApplicationDbContext dbContext, ILogger<WorkflowTemplateRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<WorkflowTemplateTable?> GetWorkflowTemplateByName(string name)
        {
            return await _dbContext.workflowTemplates.FirstOrDefaultAsync(i => i.Name == name);
        }

        public override Task<WorkflowTemplateTable> AddAsync(WorkflowTemplateTable entity)
        {
            throw new NotSupportedException("This method should not be called from this repository");
        }
        public async Task<ServerResponse<string, string>> InsertWorkflowTemplate(WorkflowTemplateTable workflowTemplate, List<WorkflowTemplateNodeTable> nodes, List<NodeTaskDependencyTable> dependencies)
        {
            // start transaction
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // insert template data                
                workflowTemplate.Id = Guid.NewGuid().ToString();
                workflowTemplate = (await _dbContext.workflowTemplates.AddAsync(workflowTemplate)).Entity;
                // update nodes to reference new workflow template Id
                foreach(var node in nodes)
                {
                    node.WorkflowTemplateId = workflowTemplate.Id;
                }
                // insert node data
                await _dbContext.workflowTemplateNodes.AddRangeAsync(nodes);
                await _dbContext.SaveChangesAsync();
                // update dependencies to reference workflow template id
                foreach(var depdency in dependencies)
                {
                    depdency.WorkflowTemplateId = workflowTemplate.Id;  
                }
                // insert node dependencies
                await _dbContext.workflowTemplateNodeDependencies.AddRangeAsync(dependencies);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();                
                return new ServerResponse<string, string>() { Success = true };
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Failed to insert workflow template data: {ex.Message}, rolledback transaction");
                return new ServerResponse<string, string>() { Success = false, Error = ex.Message };
            }
        }
    }
}
