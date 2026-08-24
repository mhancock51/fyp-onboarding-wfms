using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables;

namespace OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories
{
    public interface ITenantRepository : IRepository<TenantTable>
    {
        
    }

    public class TenantRepository : BaseRepository<TenantTable>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}