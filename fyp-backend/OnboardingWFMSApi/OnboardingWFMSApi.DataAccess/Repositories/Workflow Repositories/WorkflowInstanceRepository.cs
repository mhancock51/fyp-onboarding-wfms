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
        public Task<List<WorkflowInstanceTable>> GetInstancesBySupervisorAccountId(string supervisorAccountId);
        public Task<List<WorkflowInstanceTable>> GetInstancesByOnboarderAccountId(string onboarderAccountId);
        public Task<List<WorkflowInstanceTable>> GetInstancesWhereAccountIsAssignee(string accountId);
        public Task<List<WorkflowInstanceTable>> GetInstancesByOnboarderEmailAddress(string onboarderEmailAddress);
        public Task<List<WorkflowInstanceTable>> GetCompletedWorkflowInstances();
        public Task<List<WorkflowInstanceTable>> GetOpenWorkflowInstances();
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

        public async override Task<WorkflowInstanceTable> GetById(string id)
        {
            return await _dbContext.workflowInstances.FirstOrDefaultAsync(i => i.Id == id);            
        }

        public async Task<List<WorkflowInstanceTable>> GetInstancesBySupervisorAccountId(string supervisorAccountId)
        {
            return await _dbContext.workflowInstances.Where(i => i.SupervisorAccountId == supervisorAccountId).ToListAsync();
        }

        public async Task<List<WorkflowInstanceTable>> GetInstancesByOnboarderAccountId(string onboarderAccountId)
        {
            var workflowInstanceIds = await _dbContext.onboardingEmployeeDetails.Where(i => i.OnboarderAccountId == onboarderAccountId).Select(i => i.WorkflowInstanceId).ToListAsync();
            return await _dbContext.workflowInstances.Where(i => workflowInstanceIds.Contains(i.Id)).ToListAsync();
        }


        /// <summary>
        /// Get workflow instances where the account associated with the given account Id is an assignee of a task in a node of the workflow's template
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task<List<WorkflowInstanceTable>> GetInstancesWhereAccountIsAssignee(string accountId)
        {            
            var nodes = await _dbContext.workflowTemplateNodes.Where(i => i.AssigneeId == accountId).ToListAsync();
            return await _dbContext.workflowInstances.Where(i => nodes.Select(n => n.WorkflowTemplateId).Contains(i.WorkflowTemplateId)).ToListAsync();
        }

        public async Task<List<WorkflowInstanceTable>> GetInstancesByOnboarderEmailAddress(string onboarderEmailAddress)
        {
            var matchingEmployeeDetails = await _dbContext.onboardingEmployeeDetails.Where(i => i.EmailAddress == onboarderEmailAddress).ToListAsync();
            return await _dbContext.workflowInstances.Where(i => matchingEmployeeDetails.Select(j => j.WorkflowInstanceId).Contains(i.Id)).ToListAsync();
        }

        public async Task<List<WorkflowInstanceTable>> GetCompletedWorkflowInstances()
        {
            return await _dbContext.workflowInstances.Where(i => i.CompletionTimestamp != null).ToListAsync();
        }

        public async Task<List<WorkflowInstanceTable>> GetOpenWorkflowInstances()
        {
            return await _dbContext.workflowInstances.Where(i => i.CompletionTimestamp == null).ToListAsync();
        }
    }
}
