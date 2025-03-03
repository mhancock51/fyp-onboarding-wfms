using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IChecklistTaskTemplateRepository : IRepository<ChecklistTaskTemplateTable>
    {
        public Task<ChecklistTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId);
    }
    public class ChecklistTaskTemplateRepository : BaseRepository<ChecklistTaskTemplateTable>, IChecklistTaskTemplateRepository
    {
        public ChecklistTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ChecklistTaskTemplateTable> GetByTaskTemplateId(string id)
        {
            return await _dbContext.checklistTaskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == id);
        }
    }

}
