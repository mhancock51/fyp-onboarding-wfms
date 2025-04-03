using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
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
        public DbSet<ProjectTaskTemplateTable> projectTaskTemplates { get; set; }

        public DbSet<TaskInstanceTable> taskInstances { get; set; }
        public DbSet<ChecklistTaskInstanceTable> checklistTaskInstances { get; set; }
        public DbSet<ReadDocumentTaskInstanceTable> readDocumentTaskInstances { get; set; }
        public DbSet<FileUploadTaskInstanceTable> fileUploadTaskInstances { get; set; }
        public DbSet<ProjectTaskInstanceTable> projectTaskInstances { get; set; }

        // everything workflow related:
        public DbSet<NodeTaskDependencyTable> workflowTemplateNodeDependencies { get; set; }
        public DbSet<WorkflowTemplateNodeTable> workflowTemplateNodes { get; set; }
        public DbSet<WorkflowTemplateTable> workflowTemplates { get; set; }
        public DbSet<WorkflowInstanceTable> workflowInstances {  get; set; }
        public DbSet<OnboardingEmployeeDetailsTable> onboardingEmployeeDetails { get; set; }
        public DbSet<WorkflowInstanceNodeTable> workflowInstanceNodes { get; set; }
        
        public DbSet<CommentTable> comments { get; set; }

        public DbSet<DocumentTable> documents { get; set; }
        public DbSet<DocumentAccessLinkTable> documentAccessLinks { get; set; }

        public DbSet<ReportedIssueTable> reportedIssues { get; set; }
        public DbSet<WorkflowInstanceAuditLogTable> workflowInstanceAuditLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {             
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // make sure ProjectObjective is treated like a json property, not another table - like so for ProjectSupportLink
            modelBuilder.Entity<ProjectTaskTemplateTable>()
                .Property(e => e.Objectives)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<ProjectObjective>>(v, new JsonSerializerOptions()) ?? new List<ProjectObjective>()
                );
            modelBuilder.Entity<ProjectTaskTemplateTable>()
                .Property(e => e.SupportLinks)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<ProjectSupportLink>>(v, new JsonSerializerOptions()) ?? new List<ProjectSupportLink>()
                );

        }
    }
}
