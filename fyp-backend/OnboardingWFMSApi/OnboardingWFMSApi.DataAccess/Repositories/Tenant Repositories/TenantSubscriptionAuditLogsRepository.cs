using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;

namespace OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories
{
    public interface ITenantSubscriptionAuditLogsRepository : IRepository<TenantSubscriptionAuditLogsTable>
    {
        
    }

    public class TenantSubscriptionAuditLogsRepository : BaseRepository<TenantSubscriptionAuditLogsTable>, ITenantSubscriptionAuditLogsRepository
    {
        public TenantSubscriptionAuditLogsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}