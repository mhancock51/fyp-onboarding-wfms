using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IProjectTaskInstanceRepository : ITaskTypeInstanceRepository<ProjectTaskInstanceTable>
    {        
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
