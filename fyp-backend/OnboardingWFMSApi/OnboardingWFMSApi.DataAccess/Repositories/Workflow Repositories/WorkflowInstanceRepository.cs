using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowInstanceRepository : IRepository<WorkflowInstanceTable>
    {
        public Task<List<WorkflowInstanceTable>> GetInstancesByTemplateId(string workflowTemplateId);
    }

    public class WorkflowInstanceRepository : BaseRepository<WorkflowInstanceTable>, IWorkflowInstanceRepository
    {
        public WorkflowInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<WorkflowInstanceTable>> GetInstancesByTemplateId(string workflowTemplateId)
        {
            return await _dbContext.workflowInstances.Where(i => i.WorkflowTemplateId == workflowTemplateId).ToListAsync();
        }
    }
}
