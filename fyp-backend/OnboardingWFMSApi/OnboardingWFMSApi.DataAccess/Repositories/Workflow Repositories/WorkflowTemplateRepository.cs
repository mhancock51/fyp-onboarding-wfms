using Microsoft.EntityFrameworkCore;
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
        public Task<WorkflowTemplateTable> GetWorkflowTemplateByName(string name);
    }

    public class WorkflowTemplateRepository : BaseRepository<WorkflowTemplateTable>, IWorkflowTemplateRepository
    {
        public WorkflowTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<WorkflowTemplateTable> GetWorkflowTemplateByName(string name)
        {
            return await _dbContext.workflowTemplates.FirstOrDefaultAsync(i => i.Name == name);
        }

        public override async Task<WorkflowTemplateTable> GetById(string id)
        {
            return await _dbContext.workflowTemplates.FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
