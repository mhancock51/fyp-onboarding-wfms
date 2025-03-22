using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IDocumentAccessLinkRepository : IRepository<DocumentAccessLinkTable>
    {
        public Task<DocumentAccessLinkTable> GetAccountsAccessToResource(string accountId, string documentId);
    }

    public class DocumentAccessLinkRepository : BaseRepository<DocumentAccessLinkTable>, IDocumentAccessLinkRepository
    {
        public DocumentAccessLinkRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DocumentAccessLinkTable> GetAccountsAccessToResource(string accountId, string documentId)
        {
            return await _dbContext.documentAccessLinks.FirstOrDefaultAsync(i => i.AccountId == accountId && i.DocumentId == documentId);
        }
    }
}
