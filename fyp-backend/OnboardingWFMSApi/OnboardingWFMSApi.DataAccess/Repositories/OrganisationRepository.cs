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

    }

    public class OrganisationRepository : BaseRepository<OrganisationTable>, IOrganisationRepository
    {
        public OrganisationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<OrganisationTable> GetById(string id)
        {
            return await _dbContext.Organisations.FirstOrDefaultAsync(i => i.OrganisationId == id);
        }
    }
}
