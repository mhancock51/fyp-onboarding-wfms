using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;

namespace OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories
{
    public interface ITenantSubscriptionRepository : IRepository<TenantSubscriptionTable>
    {
        public Task<TenantSubscriptionTable?> GetTenantSubscriptionByTenantId(string tenantId);
    }

    public class TenantSubscriptionRepository : BaseRepository<TenantSubscriptionTable>, ITenantSubscriptionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TenantSubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TenantSubscriptionTable?> GetTenantSubscriptionByTenantId(string tenantId)
        {
            return await _dbContext.TenantSubscriptions
                .FirstOrDefaultAsync(t => t.TenantId == tenantId);
        }
    }
}