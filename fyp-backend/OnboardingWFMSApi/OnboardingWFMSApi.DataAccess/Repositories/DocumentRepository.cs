using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IDocumentRepository : IRepository<DocumentTable>
    {
        public Task<List<DocumentTable>> GetDocumentsFromWorkflowInstance(string workflowInstanceId);
    }
    public class DocumentRepository : BaseRepository<DocumentTable>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<DocumentTable>> GetDocumentsFromWorkflowInstance(string workflowInstanceId)
        {
            return await _dbContext.documents.Where(d => d.WorkflowInstanceId == workflowInstanceId).ToListAsync();
        }
    }

}
