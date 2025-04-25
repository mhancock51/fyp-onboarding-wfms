using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowTemplateNodeRepository : IRepository<WorkflowTemplateNodeTable>
    {
        public Task<List<WorkflowTemplateNodeTable>> GetAllNodesByWorkflowTemplateId(string workflowTemplateId);
    }
    public class WorkflowTemplateNodeRepository : BaseRepository<WorkflowTemplateNodeTable>, IWorkflowTemplateNodeRepository
    {
        public WorkflowTemplateNodeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<WorkflowTemplateNodeTable>> GetAllNodesByWorkflowTemplateId(string workflowTemplateId)
        {
            var nodes = await _dbContext.workflowTemplateNodes.Where(i => i.WorkflowTemplateId == workflowTemplateId).ToListAsync();
            return nodes.OrderBy(n => n.Order).ToList();
        }
    }
}
