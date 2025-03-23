using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IOnboardingEmployeeDetailsRepository : IRepository<OnboardingEmployeeDetailsTable>
    {
        public Task<OnboardingEmployeeDetailsTable> GetDetailsByWorkflowInstance(string workflowInstanceId);
    }
    public class OnboardingEmployeeDetailsRepository : BaseRepository<OnboardingEmployeeDetailsTable>, IOnboardingEmployeeDetailsRepository
    {
        public OnboardingEmployeeDetailsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<OnboardingEmployeeDetailsTable> GetDetailsByWorkflowInstance(string workflowInstanceId)
        {
            return await _dbContext.onboardingEmployeeDetails.FirstOrDefaultAsync(i => i.WorkflowInstanceId == workflowInstanceId);
        }
    }
}
