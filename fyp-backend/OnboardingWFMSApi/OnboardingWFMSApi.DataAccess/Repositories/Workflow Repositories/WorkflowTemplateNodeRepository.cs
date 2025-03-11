using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface IWorkflowTemplateNodeRepository : IRepository<WorkflowTemplateNodeTable>
    {

    }
    public class WorkflowTemplateNodeRepository : BaseRepository<WorkflowTemplateNodeTable>, IWorkflowTemplateNodeRepository
    {
        public WorkflowTemplateNodeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
