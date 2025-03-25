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
    }

}
