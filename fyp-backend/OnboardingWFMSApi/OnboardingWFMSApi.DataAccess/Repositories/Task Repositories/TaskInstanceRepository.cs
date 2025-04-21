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
        public Task<List<TaskInstanceTable>> GetTaskInstancesByTaskTemplateId(string taskTemplateId);
        public Task<List<TaskInstanceTable>> GetTasksCompletedPastDueDate(DateTime? from);
        public Task<List<TaskInstanceTable>> GetOverdueTasks(DateTime? from);
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

        public async Task<List<TaskInstanceTable>> GetTaskInstancesByWorkflowInstance(string workflowInstanceId)
        {
            return await _dbContext.taskInstances.Where(i => i.WorkflowInstanceId == workflowInstanceId).ToListAsync();
        }

        public async Task<List<TaskInstanceTable>> GetTaskInstancesByTaskTemplateId(string taskTemplateId)
        {
            return await _dbContext.taskInstances.Where(i => i.TaskTemplateId == taskTemplateId).ToListAsync();
        }

        public async Task<List<TaskInstanceTable>> GetTasksCompletedPastDueDate(DateTime? from)
        {
            var tasksCompletedOverdue = await _dbContext.taskInstances.Where(i => i.CompletionTimestamp != null && i.DueDate != null && i.CompletionTimestamp > i.DueDate).ToListAsync();
            if (from != null)
            {
                tasksCompletedOverdue = tasksCompletedOverdue.Where(i => i.CompletionTimestamp > from).ToList();
            }
            return tasksCompletedOverdue;
        }

        public async Task<List<TaskInstanceTable>> GetOverdueTasks(DateTime? from)
        {
            var overdueIncompleteTasks = await _dbContext.taskInstances.Where(i => i.DueDate != null && i.CompletionTimestamp == null && i.DueDate < DateTime.Now).ToListAsync();
            if (from != null)
            {
                overdueIncompleteTasks = overdueIncompleteTasks.Where(i => i.CreationTimestamp > from).ToList();
            }
            return overdueIncompleteTasks;
        }
    }

}
