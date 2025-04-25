using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IReportedIssueRepository : IRepository<ReportedIssueTable>
    {
        public Task<List<ReportedIssueTable>> GetAllOpenIssues();
    }
    public class ReportedIssueRepository : BaseRepository<ReportedIssueTable>, IReportedIssueRepository
    {
        public ReportedIssueRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ReportedIssueTable>> GetAllOpenIssues()
        {
            return await _dbContext.reportedIssues.Where(i => i.Status == "open").ToListAsync();
        }
    }
}
