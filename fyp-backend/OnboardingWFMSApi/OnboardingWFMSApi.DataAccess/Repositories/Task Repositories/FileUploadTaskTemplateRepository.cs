using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IFileUploadTaskTemplateRepository : ITaskTemplateRepository<FileUploadTaskTemplateTable>
    {
        public Task<FileUploadTaskTemplateTable> GetByTaskTemplateId(string id);
    }
    public class FileUploadTaskTemplateRepository : BaseRepository<FileUploadTaskTemplateTable>, IFileUploadTaskTemplateRepository
    {
        public FileUploadTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<FileUploadTaskTemplateTable> GetByTaskTemplateId(string id)
        {
            return await _dbContext.fileUploadTaskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == id);
        }

        public override async Task<FileUploadTaskTemplateTable> GetById(string id)
        {
            return await _dbContext.fileUploadTaskTemplates.FirstOrDefaultAsync(i => i.Id == id);            
        }

        public async Task<FileUploadTaskTemplateTable> GetByTaskInstanceId(string id)
        {
            // find checklist instance
            var taskInstance = await _dbContext.taskInstances.FirstOrDefaultAsync(i => i.Id == id);
            return await _dbContext.fileUploadTaskTemplates.FirstOrDefaultAsync(t => t.TaskTemplateId == taskInstance.TaskTemplateId);
        }
    }

}
