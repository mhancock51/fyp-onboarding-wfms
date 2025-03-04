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
    }
    public class TaskTemplateRepository : BaseRepository<TaskTemplateTable>, ITaskTemplateRepository
    {
        public TaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<TaskTemplateTable> GetById(string id)
        {
            return await _dbContext.taskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == id);
        }
    }

}
