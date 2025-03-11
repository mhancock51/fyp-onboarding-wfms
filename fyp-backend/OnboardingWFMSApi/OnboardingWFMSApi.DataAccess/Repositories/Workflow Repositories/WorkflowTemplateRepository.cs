using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowTemplateRepository : IRepository<WorkflowTemplateTable>
    {

    }

    public class WorkflowTemplateRepository : BaseRepository<WorkflowTemplateTable>, IWorkflowTemplateRepository
    {
        public WorkflowTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
