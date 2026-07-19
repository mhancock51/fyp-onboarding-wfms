using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;

namespace OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories
{
    public interface ISubscriptionTierRepository : IRepository<SubscriptionTierEntitlementTable>
    {
        
    }

    public class SubscriptionTierRepository : BaseRepository<SubscriptionTierEntitlementTable>, ISubscriptionTierRepository
    {
        public SubscriptionTierRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}