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
    }

}
