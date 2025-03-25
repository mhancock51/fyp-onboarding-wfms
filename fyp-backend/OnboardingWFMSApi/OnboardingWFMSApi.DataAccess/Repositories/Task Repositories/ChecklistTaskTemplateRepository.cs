using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IChecklistTaskTemplateRepository : ITaskTemplateRepository<ChecklistTaskTemplateTable>
    {
        public Task<ChecklistTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId);
        public Task<ChecklistTaskTemplateTable> GetByTaskInstanceId(string id);
    }
    public class ChecklistTaskTemplateRepository : BaseRepository<ChecklistTaskTemplateTable>, IChecklistTaskTemplateRepository
    {
        public ChecklistTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ChecklistTaskTemplateTable> GetByTaskInstanceId(string id)
        {
            // find checklist instance
            var taskInstance = await _dbContext.taskInstances.FirstOrDefaultAsync(i => i.Id == id);
            return await _dbContext.checklistTaskTemplates.FirstOrDefaultAsync(t => t.TaskTemplateId == taskInstance.TaskTemplateId);
        }

        public async Task<ChecklistTaskTemplateTable> GetByTaskTemplateId(string id)
        {
            return await _dbContext.checklistTaskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == id);
        }
    }

}
