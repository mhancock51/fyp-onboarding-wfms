using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IChecklistTaskInstanceRepository : IRepository<ChecklistTaskInstanceTable>
    {
        public Task<ChecklistTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId);
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

        public override async Task<ChecklistTaskInstanceTable> AddAsync(ChecklistTaskInstanceTable entity)
        {
            // DELETE ME
            throw new Exception("TEEST");
        }
    }

}
