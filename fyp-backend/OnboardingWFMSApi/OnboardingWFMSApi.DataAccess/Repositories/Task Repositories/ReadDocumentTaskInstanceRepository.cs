using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IReadDocumentTaskInstanceRepository : ITaskTypeInstanceRepository<ReadDocumentTaskInstanceTable>
    {
    }
    public class ReadDocumentTaskInstanceRepository : BaseRepository<ReadDocumentTaskInstanceTable>, IReadDocumentTaskInstanceRepository
    {
        public ReadDocumentTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<ReadDocumentTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId)
        {
            return await _dbContext.readDocumentTaskInstances.FirstOrDefaultAsync(i => i.TaskInstanceId == taskInstanceId);
        }
    }

}
