using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowNodeInstanceRepository : IRepository<WorkflowInstanceNodeTable>
    {
        public Task<List<WorkflowInstanceNodeTable>> GetByWorkflowInstanceId(string workflowInstanceId);
    }

    public class WorkflowNodeInstanceRepository : BaseRepository<WorkflowInstanceNodeTable>, IWorkflowNodeInstanceRepository
    {
        public WorkflowNodeInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<WorkflowInstanceNodeTable>> GetByWorkflowInstanceId(string workflowInstanceId)
        {
            return await _dbContext.workflowInstanceNodes.Where(i => i.WorkflowInstanceId == workflowInstanceId).ToListAsync();
        }
    }
}
