using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IProjectTaskInstanceRepository : IRepository<ProjectTaskInstanceTable>
    {
        public Task<ProjectTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId);
    }

    public class ProjectTaskInstanceRepository : BaseRepository<ProjectTaskInstanceTable>, IProjectTaskInstanceRepository
    {
        public ProjectTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ProjectTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId)
        {
            return await _dbContext.projectTaskInstances.FirstOrDefaultAsync(i => i.TaskInstanceId == taskInstanceId);
        }
    }
}
