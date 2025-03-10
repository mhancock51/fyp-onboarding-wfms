using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IFileUploadTaskInstanceRepository : IRepository<FileUploadTaskInstanceTable>
    {
        public Task<FileUploadTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId);
    }
    public class FileUploadTaskInstanceRepository : BaseRepository<FileUploadTaskInstanceTable>, IFileUploadTaskInstanceRepository
    {
        public FileUploadTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<FileUploadTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId)
        {
            return await _dbContext.fileUploadTaskInstances.FirstOrDefaultAsync(i => i.TaskInstanceId == taskInstanceId);
        }
    }

}
