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
        public Task<ServerResponse<string, string>> UpdateWorkflowTemplate(WorkflowTemplateTable workflowTemplate, List<WorkflowTemplateNodeTable> nodes, List<NodeTaskDependencyTable> dependencies);
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
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            ServerResponse<string, string> result = new() { Success = false, Error = "Insert workflow template transaction was not executed." };

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // insert template data
                    workflowTemplate.Id = Guid.NewGuid().ToString();
                    workflowTemplate = (await _dbContext.workflowTemplates.AddAsync(workflowTemplate)).Entity;

                    // update nodes to reference new workflow template Id
                    foreach (var node in nodes)
                    {
                        node.WorkflowTemplateId = workflowTemplate.Id;
                    }

                    // insert node data
                    await _dbContext.workflowTemplateNodes.AddRangeAsync(nodes);
                    await _dbContext.SaveChangesAsync();

                    // update dependencies to reference workflow template id
                    foreach (var depdency in dependencies)
                    {
                        depdency.WorkflowTemplateId = workflowTemplate.Id;
                    }

                    // insert node dependencies
                    await _dbContext.workflowTemplateNodeDependencies.AddRangeAsync(dependencies);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    result = new ServerResponse<string, string>() { Success = true };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Failed to insert workflow template: {ex.Message}, rolledback transaction");
                    result = new ServerResponse<string, string>() { Success = false, Error = ex.Message };
                }
            });

            return result;
        }

        public async Task<ServerResponse<string, string>> UpdateWorkflowTemplate(WorkflowTemplateTable workflowTemplate, List<WorkflowTemplateNodeTable> nodes, List<NodeTaskDependencyTable> dependencies)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            ServerResponse<string, string> result = new() { Success = false, Error = "Update workflow template transaction was not executed." };

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                try
                {
                    // update base template data
                    _dbContext.workflowTemplates.Update(workflowTemplate);
                    await _dbContext.SaveChangesAsync();

                    // UPDATE NODES
                    // get existing nodes
                    var existingNodes = _dbContext.workflowTemplateNodes.Where(n => n.WorkflowTemplateId == workflowTemplate.Id);
                    var updatedExistingNodes = nodes.Where(n => existingNodes.Contains(n));
                    // update data of nodes that already exist
                    _dbContext.workflowTemplateNodes.UpdateRange(updatedExistingNodes);
                    await _dbContext.SaveChangesAsync();

                    // find new nodes and insert
                    var newNodes = nodes.Where(n => !existingNodes.Contains(n));
                    await _dbContext.workflowTemplateNodes.AddRangeAsync(newNodes);
                    await _dbContext.SaveChangesAsync();
                    // remove deleted nodes
                    var deletedNodes = existingNodes.Where(n => !nodes.Contains(n));
                    _dbContext.workflowTemplateNodes.RemoveRange(deletedNodes);
                    await _dbContext.SaveChangesAsync();

                    // UPDATE DEPENDENCIES
                    // get existing dependencies
                    var existingDependencies = _dbContext.workflowTemplateNodeDependencies.Where(d => d.WorkflowTemplateId == workflowTemplate.Id);
                    var updatedExistingDependencies = dependencies.Where(d => existingDependencies.Contains(d));
                    // update data of dependencies that already exist
                    _dbContext.workflowTemplateNodeDependencies.UpdateRange(updatedExistingDependencies);
                    await _dbContext.SaveChangesAsync();

                    // find new dependencies and insert
                    var newDependencies = dependencies.Where(d => !existingDependencies.Contains(d));
                    await _dbContext.workflowTemplateNodeDependencies.AddRangeAsync(newDependencies);
                    await _dbContext.SaveChangesAsync();
                    // remove deleted dependencies
                    var deletedDependencies = existingDependencies.Where(n => !dependencies.Contains(n));
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    result = new ServerResponse<string, string>() { Success = true };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Failed to update workflow template: {ex.Message}");
                    result = new ServerResponse<string, string>() { Success = false, Error = ex.Message };
                }
            });

            return result;
        }
    }
}
