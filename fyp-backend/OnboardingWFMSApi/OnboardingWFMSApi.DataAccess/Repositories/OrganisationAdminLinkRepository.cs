using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IOrganisationAdminLinkRepository : IRepository<OrganisationAdminLinkTable>
    {

    }
    public class OrganisationAdminLinkRepository : BaseRepository<OrganisationAdminLinkTable>, IOrganisationAdminLinkRepository
    {
        public OrganisationAdminLinkRepository(ApplicationDbContext dbContext) : base(dbContext)
        {            
        }

        public override Task AddAsync(OrganisationAdminLinkTable entity)
        {
            // only allow one admin
            if (_dbContext.OrganisationAdmins.FirstOrDefault(i => i.OrganisationId == entity.OrganisationId) != null)
            {
                throw new Exception("Organisation already has admin");
            }
            else
            {                
                return base.AddAsync(entity);
            }
        }
    }
}
