using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<OrganisationTable> Organisations { get; set; }
        public DbSet<AccountTable> Accounts { get; set; }
        public DbSet<DepartmentTable> Departments { get; set; }
        public DbSet<OrganisationAdminLinkTable> OrganisationAdmins { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
