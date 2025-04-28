using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IDecisionTaskTemplateRepository : ITaskTemplateRepository<DecisionTaskTemplateTable>
    {

    }

    public class DecisionTaskTemplateRepository : BaseRepository<DecisionTaskTemplateTable>, IDecisionTaskTemplateRepository
    {
        public DecisionTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DecisionTaskTemplateTable> GetByTaskInstanceId(string id)
        {
            return await _dbContext.decisionTaskTemplates.FirstOrDefaultAsync(i => i.TaskTemplateId == id);
        }

        public async Task<DecisionTaskTemplateTable> GetByTaskTemplateId(string id)
        {
            // find decision instance
            var decisionInstance = await _dbContext.decisionTaskInstances.FirstOrDefaultAsync(i => i.Id == id);
            return await _dbContext.decisionTaskTemplates.FirstOrDefaultAsync(t => t.TaskTemplateId == decisionInstance.TaskTemplateId);
        }
    }
}
