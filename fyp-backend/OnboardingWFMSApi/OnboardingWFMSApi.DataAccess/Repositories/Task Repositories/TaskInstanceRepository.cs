using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface ITaskInstanceRepository : IRepository<TaskInstanceTable>
    {
        public Task<List<TaskInstanceTable>> GetUsersTaskInstances(string accountId);
        public Task<List<TaskInstanceTable>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId);
    }
    public class TaskInstanceRepository : BaseRepository<TaskInstanceTable>, ITaskInstanceRepository
    {
        public TaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<TaskInstanceTable>> GetUsersTaskInstances(string accountId)
        {
            return _dbContext.taskInstances.Where(i => i.AssigneeAccountId == accountId).ToList();
        }

        public override async Task<TaskInstanceTable> GetById(string id)
        {
            return await _dbContext.taskInstances.FirstOrDefaultAsync(i => i.Id == id);            
        }

        public async Task<List<TaskInstanceTable>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId)
        {
            return await _dbContext.taskInstances.Where(i => i.WorkflowInstanceId == workflowInstanceId).ToListAsync();
        }
    }

}
