using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface INodeTaskDependencyRepository : IRepository<NodeTaskDependencyTable>
    {
        public Task<List<NodeTaskDependencyTable>> GetAllNodeDependenciesByWorkflowTemplateId(string workflowTemplateId);
    }
    public class NodeTaskDependencyRepository : BaseRepository<NodeTaskDependencyTable>, INodeTaskDependencyRepository
    {
        public NodeTaskDependencyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<NodeTaskDependencyTable>> GetAllNodeDependenciesByWorkflowTemplateId(string workflowTemplateId)
        {
            return await _dbContext.workflowTemplateNodeDependencies.Where(i => i.WorkflowTemplateId == workflowTemplateId).ToListAsync();
        }
    }
}
