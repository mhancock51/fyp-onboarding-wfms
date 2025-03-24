using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IProjectTaskTemplateRepository : IRepository<ProjectTaskTemplateTable>
    {
        public Task<ProjectTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId);
    }

    public class ProjectTaskTemplateRepository : BaseRepository<ProjectTaskTemplateTable>, IProjectTaskTemplateRepository
    {
        public ProjectTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ProjectTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId)
        {
            return await _dbContext.projectTaskTemplates.FirstOrDefaultAsync(t => t.TaskTemplateId == taskTemplateId);
        }
    }
}
