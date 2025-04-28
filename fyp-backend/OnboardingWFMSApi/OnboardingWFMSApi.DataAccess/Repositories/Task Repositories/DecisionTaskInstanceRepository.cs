using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories.Interfaces
{
    public interface IDecisionTaskInstanceRepository : ITaskTypeInstanceRepository<DecisionTaskInstanceTable>
    {

    }

    public class DecisionTaskInstanceRepository : BaseRepository<DecisionTaskInstanceTable>, IDecisionTaskInstanceRepository
    {
        public DecisionTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DecisionTaskInstanceTable> GetByTaskInstanceId(string taskInstanceId)
        {
            return await _dbContext.decisionTaskInstances.FirstOrDefaultAsync(i => i.TaskInstanceId == taskInstanceId);
        }
    }
}
