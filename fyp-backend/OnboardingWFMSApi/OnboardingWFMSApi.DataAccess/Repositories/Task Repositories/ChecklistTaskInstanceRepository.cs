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
    public interface IChecklistTaskInstanceRepository : ITaskTypeInstanceRepository<ChecklistTaskInstanceTable>
    {        
    }
    public class ChecklistTaskInstanceRepository : BaseRepository<ChecklistTaskInstanceTable>, IChecklistTaskInstanceRepository
    {
        public ChecklistTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ChecklistTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId)
        {
            return await _dbContext.checklistTaskInstances.FirstOrDefaultAsync(i => i.TaskInstanceId == taskInstanceId);
        }
    }

}
