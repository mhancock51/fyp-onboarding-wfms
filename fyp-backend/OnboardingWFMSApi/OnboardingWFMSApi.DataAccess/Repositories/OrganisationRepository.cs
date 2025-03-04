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
            var org = await _dbContext.Organisations.FirstOrDefaultAsync(i => i.OrganisationId == id);
            return org != null ? true : false;
        }

        public override async Task<OrganisationTable> GetById(string id)
        {
            return await _dbContext.Organisations.FirstOrDefaultAsync(i => i.OrganisationId == id);
        }

        public override async Task<OrganisationTable> AddAsync(OrganisationTable entity)
        {
            if (_dbContext.Organisations.Count() > 0)
            {
                throw new Exception("Only one organisation can exist");
            }
            else
            {
                return await base.AddAsync(entity);
            }

        }

        public async Task<int> GetNumberOfOrganisations()
        {
            return _dbContext.Organisations.Count();
        }
    }
}
