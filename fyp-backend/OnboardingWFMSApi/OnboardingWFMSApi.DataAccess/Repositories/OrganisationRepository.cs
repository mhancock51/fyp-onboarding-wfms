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

        public 
    }
}
