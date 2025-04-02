using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface ITaskTemplateRepository : IRepository<TaskTemplateTable>
    {
        public Task<List<TaskTemplateTable>> GetAllByStatus(string status);
    }
    public class TaskTemplateRepository : BaseRepository<TaskTemplateTable>, ITaskTemplateRepository
    {
        public TaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<TaskTemplateTable>> GetAllByStatus(string status)
        {
            return await _dbContext.taskTemplates.Where(t => t.Status.ToLower() == status.ToLower()).ToListAsync();
        }
    }

}
