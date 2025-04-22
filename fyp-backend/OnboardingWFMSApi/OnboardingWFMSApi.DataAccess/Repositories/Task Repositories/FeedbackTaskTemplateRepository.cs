using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IFeedbackTaskTemplateRepository : ITaskTemplateRepository<FeedbackTaskTemplateTable>
    {

    }

    public class FeedbackTaskTemplateRepository : BaseRepository<FeedbackTaskTemplateTable>, IFeedbackTaskTemplateRepository
    {
        public FeedbackTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<FeedbackTaskTemplateTable> GetByTaskInstanceId(string id)
        {
            // find feedback instance
            var taskInstance = await _dbContext.taskInstances.FirstOrDefaultAsync(i => i.Id == id);
            return await _dbContext.feedbackTaskTemplates.FirstOrDefaultAsync(t => t.TaskTemplateId == taskInstance.TaskTemplateId);
        }

        public async Task<FeedbackTaskTemplateTable> GetByTaskTemplateId(string id)
        {
            return await _dbContext.feedbackTaskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == id);
        }
    }
}
