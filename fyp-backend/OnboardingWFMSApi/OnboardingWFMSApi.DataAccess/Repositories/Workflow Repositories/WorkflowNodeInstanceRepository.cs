using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowNodeInstanceRepository : IRepository<WorkflowInstanceNodeTable>
    {

    }

    public class WorkflowNodeInstanceRepository : BaseRepository<WorkflowInstanceNodeTable>, IWorkflowNodeInstanceRepository
    {
        public WorkflowNodeInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
