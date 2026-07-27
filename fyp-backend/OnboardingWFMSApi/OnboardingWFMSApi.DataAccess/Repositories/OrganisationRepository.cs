using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IOrganisationRepository : IRepository<OrganisationTable>
    {
        public Task<int> GetNumberOfOrganisations();
    }

    public class OrganisationRepository : BaseRepository<OrganisationTable>, IOrganisationRepository
    {
        public OrganisationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<bool> ExistsById(string id)
        {
            var org = await _dbContext.Organisations.FirstOrDefaultAsync(i => i.Id == id);
            return org != null ? true : false;
        }

        public override async Task<OrganisationTable> AddAsync(OrganisationTable entity)
        {
            // Multi-tenancy: each tenant may have its own organisation.
            // Only enforce that a given tenant has at most one organisation.
            if (!string.IsNullOrWhiteSpace(entity.TenantId))
            {
                var existingForTenant = await _dbContext.Organisations
                    .FirstOrDefaultAsync(o => o.TenantId == entity.TenantId);
                if (existingForTenant != null)
                {
                    throw new Exception($"An organisation already exists for tenant {entity.TenantId}");
                }
            }

            return await base.AddAsync(entity);
        }

        public async Task<int> GetNumberOfOrganisations()
        {
            return _dbContext.Organisations.Count();
        }
    }
}
