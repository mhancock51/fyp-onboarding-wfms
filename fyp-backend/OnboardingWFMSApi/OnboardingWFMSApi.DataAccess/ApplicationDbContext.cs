using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ICurrentTenantService _currentTenantService;
        private string? CurrentTenantId => _currentTenantService.TenantId;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentTenantService currentTenantService) : base(options)
        {
            _currentTenantService = currentTenantService;
        }

        // SaaS tenancy data management
        public DbSet<TenantTable> Tennants { get; set; }
        public DbSet<SubscriptionTierEntitlementTable> SubscriptionTierEntitlements { get; set; } 

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
        public DbSet<FeedbackTaskTemplateTable> feedbackTaskTemplates { get; set; }

        public DbSet<TaskInstanceTable> taskInstances { get; set; }
        public DbSet<ChecklistTaskInstanceTable> checklistTaskInstances { get; set; }
        public DbSet<ReadDocumentTaskInstanceTable> readDocumentTaskInstances { get; set; }
        public DbSet<FileUploadTaskInstanceTable> fileUploadTaskInstances { get; set; }
        public DbSet<ProjectTaskInstanceTable> projectTaskInstances { get; set; }
        public DbSet<FeedbackTaskInstanceTable> feedbackTaskInstances { get; set; }  

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

        public DbSet<NotificationTable> notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                        .Where(t => typeof(ITenantTableEntity).IsAssignableFrom(t.ClrType)))
            {
                var method = typeof(ApplicationDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { modelBuilder });
            }

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
            // make sure LikertQuestion is treated like a njson property, not another table
            modelBuilder.Entity<FeedbackTaskTemplateTable>()
               .Property(e => e.Questions)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                   v => JsonSerializer.Deserialize<List<LikertQuestion>>(v, new JsonSerializerOptions()) ?? new List<LikertQuestion>()
               );

            // make tentant tables unique
            modelBuilder.Entity<TenantTable>()
                .HasIndex(t => t.OwnerEmailAddress)
                .IsUnique();

        }

        private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder)
            where TEntity : class, ITenantTableEntity
        {
            modelBuilder.Entity<TEntity>()
                .HasQueryFilter(e => e.TenantId == CurrentTenantId);
        }
    }
}
