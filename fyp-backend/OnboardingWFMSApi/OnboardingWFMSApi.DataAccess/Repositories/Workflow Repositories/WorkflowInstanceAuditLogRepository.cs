using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowInstanceAuditLogRepository : IRepository<WorkflowInstanceAuditLogTable>
    {
        public Task<List<WorkflowInstanceAuditLogTable>> GetAuditLogsByWorkflowInstanceId(string workflowInstanceId);
    }

    public class WorkflowInstanceAuditLogRepository : BaseRepository<WorkflowInstanceAuditLogTable>, IWorkflowInstanceAuditLogRepository
    {
        public WorkflowInstanceAuditLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<WorkflowInstanceAuditLogTable>> GetAuditLogsByWorkflowInstanceId(string workflowInstanceId)
        {
            return await _dbContext.workflowInstanceAuditLogs.Where(l => l.WorkflowInstanceId == workflowInstanceId).ToListAsync();
        }
    }
}
