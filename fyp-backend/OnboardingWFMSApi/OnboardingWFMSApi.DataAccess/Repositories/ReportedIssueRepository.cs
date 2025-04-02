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

    }
    public class ReportedIssueRepository : BaseRepository<ReportedIssueTable>, IReportedIssueRepository
    {
        public ReportedIssueRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
