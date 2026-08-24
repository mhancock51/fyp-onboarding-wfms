using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IReadDocumentTaskTemplateRepository : ITaskTemplateRepository<ReadDocumentTaskTemplateTable>
    {        
    }
    public class ReadDocumentTaskTemplateRepository : BaseRepository<ReadDocumentTaskTemplateTable>, IReadDocumentTaskTemplateRepository
    {
        public ReadDocumentTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ReadDocumentTaskTemplateTable> GetByTaskInstanceId(string id)
        {
            // find checklist instance
            var taskInstance = await _dbContext.taskInstances.FirstOrDefaultAsync(i => i.Id == id);
            return await _dbContext.readDocumentTaskTemplates.FirstOrDefaultAsync(t => t.TaskTemplateId == taskInstance.TaskTemplateId);
        }

        public async Task<ReadDocumentTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId)
        {
            return await _dbContext.readDocumentTaskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == taskTemplateId);            
        }
    }

}
