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
    public interface IFeedbackTaskInstanceRepository : ITaskTypeInstanceRepository<FeedbackTaskInstanceTable>
    {

    }

    public class FeedbackTaskInstanceRepository : BaseRepository<FeedbackTaskInstanceTable>, IFeedbackTaskInstanceRepository
    {
        public FeedbackTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<FeedbackTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId)
        {
            return await _dbContext.feedbackTaskInstances.FirstOrDefaultAsync(i => i.TaskInstanceId == taskInstanceId);
        }
    }
}
