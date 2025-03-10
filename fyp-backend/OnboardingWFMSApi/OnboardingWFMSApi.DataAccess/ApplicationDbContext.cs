using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
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
        // Everything task related:
        public DbSet<TaskTypeTable> taskTypes{ get; set; }
        public DbSet<TaskTemplateTable> taskTemplates { get; set; }
        public DbSet<FileUploadTaskTemplateTable> fileUploadTaskTemplates { get; set; }
        public DbSet<ReadDocumentTaskTemplateTable> readDocumentTaskTemplates { get; set; }
        public DbSet<ChecklistTaskTemplateTable> checklistTaskTemplates { get; set; }

        public DbSet<TaskInstanceTable> taskInstances { get; set; }
        public DbSet<ChecklistTaskInstanceTable> checklistTaskInstances { get; set; }
        public DbSet<ReadDocumentTaskInstanceTable> readDocumentTaskInstances { get; set; }

        public DbSet<DocumentTable> documents { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
