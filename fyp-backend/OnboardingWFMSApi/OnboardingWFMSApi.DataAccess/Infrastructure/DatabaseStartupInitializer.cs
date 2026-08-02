using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace OnboardingWFMSApi.DataAccess.Infrastructure
{
    public static class DatabaseStartupInitializer
    {
        private const string DefaultAdminPlainPassword = "pword123";
        private const string DefaultNonAdminPlainPassword = "pword123";
        private const string RegisteredAccountStatus = "registered";

        private static readonly TenantSeedDefinition[] DefaultTenantSeeds =
        {
            new()
            {
                TenantId = "default-tenant-1",
                OrganisationId = "my-org-1",
                OrganisationName = "My Org 1",
                DepartmentId = "default-department-1",
                DepartmentName = "General",
                AdminAccountId = "default-supervisor-admin-1",
                AdminDisplayName = "Default Supervisor 1",
                AdminEmail = "admin1@test.co.uk",
                NonAdminAccounts = new[]
                {
                    new NonAdminSeedDefinition
                    {
                        AccountId = "default-user-1",
                        DisplayName = "Default User 1",
                        Email = "user1@test.co.uk"
                    }
                }
            },
            new()
            {
                TenantId = "default-tenant-2",
                OrganisationId = "my-org-2",
                OrganisationName = "My Org 2",
                DepartmentId = "default-department-2",
                DepartmentName = "General",
                AdminAccountId = "default-supervisor-admin-2",
                AdminDisplayName = "Default Supervisor 2",
                AdminEmail = "admin2@test.co.uk",
                NonAdminAccounts = Array.Empty<NonAdminSeedDefinition>()
            },
        };

        private static readonly SubscriptionTierEntitlementTable[] defaultSubscriptionTiers =
        {
            new SubscriptionTierEntitlementTable()
            {
                Id = "admin-tier",
                DisplayName = "Admin tier",
                CanUploadDocuments = true,
                MaxActiveWorkflowInstances = 99,
                MaxDocumentStorageSpaceInMb = 999999,
                MaxUsers = 999,
                CreatedDate = DateTime.Now,
                IsActive = true,
                PriceId = "",
            },
            new SubscriptionTierEntitlementTable()
            {
                Id = "tier-1-subscription",
                DisplayName = "Starter Tier",
                CanUploadDocuments = true,
                MaxActiveWorkflowInstances = 3,
                MaxDocumentStorageSpaceInMb = 5000,
                MaxUsers = 5,
                CreatedDate = DateTime.Now,
                IsActive = true,
                PriceId = "price_1TvHoUF3XLGavbWV0MF0ZXIW",
            },
            new SubscriptionTierEntitlementTable()
            {
                Id = "tier-2-subscription",
                DisplayName = "Professional Tier",
                CanUploadDocuments = true,
                MaxActiveWorkflowInstances = 15,
                MaxDocumentStorageSpaceInMb = 50000,
                MaxUsers = 5,
                CreatedDate = DateTime.Now,
                IsActive = true,
                PriceId = "price_1TvHolF3XLGavbWVTeoOaQar",
            },
            new SubscriptionTierEntitlementTable()
            {
                Id = "tier-3-subscription",
                DisplayName = "Enterprise Tier",
                CanUploadDocuments = true,
                MaxActiveWorkflowInstances = 9999,
                MaxDocumentStorageSpaceInMb = 500000,
                MaxUsers = 5,
                CreatedDate = DateTime.Now,
                IsActive = true,
                PriceId = "price_1TvHowF3XLGavbWVvEe7buh3",
            }
        };

        private static readonly (string Id, string Name)[] DefaultTaskTypes =
        {
            ("checklist", "Checklist"),
            ("upload-document", "Upload Document"),
            ("read-document", "Read Document"),
            ("project-task", "Project Task"),
            ("feedback", "Feedback Task")
        };

        // ── Seed Task Template Definitions ──────────────────────────────────
        // Each definition describes a task template + its type-specific data.
        // Tenant-scoped IDs are generated at seed time as: {tenantId}-{suffix}

        private sealed class SeedTaskTemplateDef
        {
            public required string IdSuffix { get; init; }
            public required string Name { get; init; }
            public required string Description { get; init; }
            public required string TaskTypeId { get; init; }  // references DefaultTaskTypes Id
            public required object TypeSpecificData { get; init; }
        }

        private static readonly SeedTaskTemplateDef[] DefaultTaskTemplateDefs =
        {
            // ── Checklists ──────────────────────────────────────────────
            new()
            {
                IdSuffix = "tt-checklist-paperwork",
                Name = "New Hire Paperwork",
                Description = "Complete all required paperwork for new employee onboarding.",
                TaskTypeId = "checklist",
                TypeSpecificData = new ChecklistTaskTemplateTable
                {
                    Items = new[]
                    {
                        "Signed employment contract",
                        "Emergency contact information form",
                        "Tax withholding form (W-4 / equivalent)",
                        "Direct deposit / bank details form",
                        "Right-to-work / I-9 eligibility verification",
                        "Benefits enrolment form",
                        "Confidentiality / NDA agreement"
                    }
                }
            },
            new()
            {
                IdSuffix = "tt-checklist-it-setup",
                Name = "IT Equipment Setup",
                Description = "Ensure all IT equipment and accounts are provisioned for the new starter.",
                TaskTypeId = "checklist",
                TypeSpecificData = new ChecklistTaskTemplateTable
                {
                    Items = new[]
                    {
                        "Laptop / desktop assigned and imaged",
                        "Email account created and tested",
                        "VPN / remote access configured",
                        "Required software installed and licensed",
                        "Phone extension / VoIP account assigned",
                        "Building access card / fob issued",
                        "Monitor, keyboard, mouse and peripherals set up"
                    }
                }
            },

            // ── Upload Documents ────────────────────────────────────────
            new()
            {
                IdSuffix = "tt-upload-contract",
                Name = "Upload Signed Contract",
                Description = "Upload the countersigned employment contract for HR records.",
                TaskTypeId = "upload-document",
                TypeSpecificData = new FileUploadTaskTemplateTable
                {
                    SupportedDocumentType = "pdf",
                    DocumentName = "Employment Contract",
                    AccessAccountIds = Array.Empty<string>()
                }
            },
            new()
            {
                IdSuffix = "tt-upload-id",
                Name = "Upload ID Documents",
                Description = "Upload a scan or photo of passport, driver's licence, or national ID for right-to-work verification.",
                TaskTypeId = "upload-document",
                TypeSpecificData = new FileUploadTaskTemplateTable
                {
                    SupportedDocumentType = "pdf/jpg/png",
                    DocumentName = "ID Document",
                    AccessAccountIds = Array.Empty<string>()
                }
            },

            // ── Read Documents ──────────────────────────────────────────
            new()
            {
                IdSuffix = "tt-read-handbook",
                Name = "Company Handbook",
                Description = "Read the employee handbook and confirm you have understood its contents.",
                TaskTypeId = "read-document",
                TypeSpecificData = new ReadDocumentTaskTemplateTable
                {
                    DocumentName = "Company Handbook",
                    DocumentUrl = "",
                    CheckBoxLabel = "I have read and understood the Company Handbook."
                }
            },
            new()
            {
                IdSuffix = "tt-read-it-policy",
                Name = "IT Security Policy",
                Description = "Read the IT security and acceptable-use policy and confirm your agreement.",
                TaskTypeId = "read-document",
                TypeSpecificData = new ReadDocumentTaskTemplateTable
                {
                    DocumentName = "IT Security Policy",
                    DocumentUrl = "",
                    CheckBoxLabel = "I agree to comply with the IT Security and Acceptable Use Policy."
                }
            },
            new()
            {
                IdSuffix = "tt-read-health-safety",
                Name = "Health & Safety Policy",
                Description = "Read the workplace health and safety guidelines.",
                TaskTypeId = "read-document",
                TypeSpecificData = new ReadDocumentTaskTemplateTable
                {
                    DocumentName = "Health & Safety Guidelines",
                    DocumentUrl = "",
                    CheckBoxLabel = "I have read and understood the Health & Safety guidelines."
                }
            },

            // ── Project Task ────────────────────────────────────────────
            new()
            {
                IdSuffix = "tt-project-first-week",
                Name = "First Week Starter Project",
                Description = "Complete a small introductory project to get familiar with tools, team, and workflows.",
                TaskTypeId = "project-task",
                TypeSpecificData = new ProjectTaskTemplateTable
                {
                    Brief = "This starter project helps you get comfortable with our tools, team, and development workflow. You will set up your local environment, explore the codebase, and make a small contribution.",
                    Deliverable = "A working local development environment and one small code or documentation contribution merged to the repository.",
                    Objectives = new List<ProjectObjective>
                    {
                        new() { Id = "obj-setup-env", Objective = "Set up local development environment", Required = true },
                        new() { Id = "obj-clone-run", Objective = "Clone the main repository and run the test suite successfully", Required = true },
                        new() { Id = "obj-standup", Objective = "Attend at least one team standup or sync meeting", Required = true },
                        new() { Id = "obj-contribute", Objective = "Make a small documentation update or low-risk bug fix", Required = false }
                    },
                    Skills = new List<string> { "Git", "VS Code / IDE", "Team Communication" },
                    SupportLinks = new List<ProjectSupportLink>
                    {
                        new() { Id = "link-setup-guide", Description = "Step-by-step development environment setup guide", LinkLabel = "Setup Guide", Link = "" },
                        new() { Id = "link-team-chat", Description = "Join the team chat channel for questions", LinkLabel = "Team Chat", Link = "" }
                    }
                }
            },

            // ── Feedback Surveys ────────────────────────────────────────
            new()
            {
                IdSuffix = "tt-feedback-30day",
                Name = "30-Day Check-in Survey",
                Description = "Complete a brief survey about your first 30 days so we can continually improve.",
                TaskTypeId = "feedback",
                TypeSpecificData = new FeedbackTaskTemplateTable
                {
                    Questions = new List<LikertQuestion>
                    {
                        new() { Question = "How would you rate your overall onboarding experience?", LikertScale = new[] { "Very Poor", "Poor", "Neutral", "Good", "Excellent" } },
                        new() { Question = "Do you feel you have the tools and resources needed to do your job effectively?", LikertScale = new[] { "Strongly Disagree", "Disagree", "Neutral", "Agree", "Strongly Agree" } },
                        new() { Question = "How well do you understand your role and responsibilities?", LikertScale = new[] { "Not at All", "Slightly", "Moderately", "Well", "Very Well" } },
                        new() { Question = "How welcomed did you feel by your team?", LikertScale = new[] { "Not Welcomed", "Slightly Welcomed", "Moderately Welcomed", "Welcomed", "Very Welcomed" } }
                    }
                }
            },
            new()
            {
                IdSuffix = "tt-feedback-experience",
                Name = "Onboarding Experience Survey",
                Description = "Help us improve the onboarding process by sharing your honest feedback.",
                TaskTypeId = "feedback",
                TypeSpecificData = new FeedbackTaskTemplateTable
                {
                    Questions = new List<LikertQuestion>
                    {
                        new() { Question = "Was the onboarding timeline clearly communicated?", LikertScale = new[] { "Not at All", "Slightly", "Moderately", "Mostly", "Completely" } },
                        new() { Question = "How helpful was your buddy or mentor during onboarding?", LikertScale = new[] { "Not Helpful", "Slightly Helpful", "Moderately Helpful", "Helpful", "Extremely Helpful" } },
                        new() { Question = "What area could we improve the most?", LikertScale = new[] { "Nothing — it was great", "Documentation", "Timeline / Pacing", "Communication", "Training Materials" } }
                    }
                }
            },

            // ── Expense Report ────────────────────────────────────────
            new()
            {
                IdSuffix = "tt-upload-receipts",
                Name = "Upload Receipts",
                Description = "Upload scanned or digital receipts for expense reimbursement.",
                TaskTypeId = "upload-document",
                TypeSpecificData = new FileUploadTaskTemplateTable
                {
                    SupportedDocumentType = "pdf/jpg/png",
                    DocumentName = "Expense Receipt",
                    AccessAccountIds = Array.Empty<string>()
                }
            },
            new()
            {
                IdSuffix = "tt-read-expense-policy",
                Name = "Expense Reimbursement Policy",
                Description = "Read the company expense reimbursement policy and confirm understanding.",
                TaskTypeId = "read-document",
                TypeSpecificData = new ReadDocumentTaskTemplateTable
                {
                    DocumentName = "Expense Reimbursement Policy",
                    DocumentUrl = "",
                    CheckBoxLabel = "I have read and agree to comply with the Expense Reimbursement Policy."
                }
            },
            new()
            {
                IdSuffix = "tt-checklist-expense-items",
                Name = "Expense Report Itemisation",
                Description = "Itemise each expense with date, amount, category, and business justification.",
                TaskTypeId = "checklist",
                TypeSpecificData = new ChecklistTaskTemplateTable
                {
                    Items = new[]
                    {
                        "Date of expense recorded for each item",
                        "Amount and currency specified for each item",
                        "Expense category assigned (travel, meals, supplies, etc.)",
                        "Business purpose / justification provided",
                        "Receipt attached for each item over the threshold",
                        "Total amount calculated and verified"
                    }
                }
            },
            new()
            {
                IdSuffix = "tt-project-manager-approval",
                Name = "Manager Expense Approval",
                Description = "Manager reviews the expense report, verifies compliance, and approves or rejects.",
                TaskTypeId = "project-task",
                TypeSpecificData = new ProjectTaskTemplateTable
                {
                    Brief = "Review the submitted expense report for policy compliance, accuracy, and appropriate business justification. Approve or request corrections.",
                    Deliverable = "Approved expense report forwarded to finance, or returned with comments for correction.",
                    Objectives = new List<ProjectObjective>
                    {
                        new() { Id = "obj-verify-receipts", Objective = "Verify all required receipts are attached and legible", Required = true },
                        new() { Id = "obj-check-policy", Objective = "Confirm each expense complies with company policy", Required = true },
                        new() { Id = "obj-verify-amounts", Objective = "Validate totals and individual amounts are correct", Required = true },
                        new() { Id = "obj-approve-reject", Objective = "Approve the report or return it with feedback", Required = true }
                    },
                    Skills = new List<string> { "Policy Knowledge", "Attention to Detail" },
                    SupportLinks = new List<ProjectSupportLink>
                    {
                        new() { Id = "link-expense-policy", Description = "Full expense reimbursement policy document", LinkLabel = "Expense Policy", Link = "" },
                        new() { Id = "link-finance-contact", Description = "Contact finance for policy clarification", LinkLabel = "Finance Team", Link = "" }
                    }
                }
            },

            // ── Performance Review ────────────────────────────────────
            new()
            {
                IdSuffix = "tt-read-perf-review-policy",
                Name = "Performance Review Guidelines",
                Description = "Read the performance review process, rating scale, and timeline guidelines.",
                TaskTypeId = "read-document",
                TypeSpecificData = new ReadDocumentTaskTemplateTable
                {
                    DocumentName = "Performance Review Guidelines",
                    DocumentUrl = "",
                    CheckBoxLabel = "I have read and understood the Performance Review process and rating criteria."
                }
            },
            new()
            {
                IdSuffix = "tt-feedback-self-assessment",
                Name = "Performance Self-Assessment",
                Description = "Complete a self-assessment reflecting on achievements, challenges, and growth over the review period.",
                TaskTypeId = "feedback",
                TypeSpecificData = new FeedbackTaskTemplateTable
                {
                    Questions = new List<LikertQuestion>
                    {
                        new() { Question = "How would you rate your overall performance during this review period?", LikertScale = new[] { "Needs Improvement", "Meets Some Expectations", "Meets Expectations", "Exceeds Expectations", "Outstanding" } },
                        new() { Question = "How effectively did you achieve your goals from the previous review?", LikertScale = new[] { "Did Not Achieve", "Partially Achieved", "Mostly Achieved", "Fully Achieved", "Exceeded" } },
                        new() { Question = "How well did you collaborate and communicate with your team?", LikertScale = new[] { "Needs Improvement", "Adequate", "Good", "Very Good", "Excellent" } },
                        new() { Question = "How would you rate your professional development and skill growth?", LikertScale = new[] { "No Growth", "Minimal Growth", "Moderate Growth", "Significant Growth", "Exceptional Growth" } }
                    }
                }
            },
            new()
            {
                IdSuffix = "tt-feedback-manager-assessment",
                Name = "Manager Performance Assessment",
                Description = "Manager completes a structured assessment of the employee's performance, contributions, and areas for development.",
                TaskTypeId = "feedback",
                TypeSpecificData = new FeedbackTaskTemplateTable
                {
                    Questions = new List<LikertQuestion>
                    {
                        new() { Question = "How would you rate the employee's overall performance?", LikertScale = new[] { "Needs Improvement", "Meets Some Expectations", "Meets Expectations", "Exceeds Expectations", "Outstanding" } },
                        new() { Question = "How consistently does the employee demonstrate the company values?", LikertScale = new[] { "Rarely", "Sometimes", "Often", "Almost Always", "Always" } },
                        new() { Question = "How would you rate the quality and timeliness of the employee's work?", LikertScale = new[] { "Needs Improvement", "Adequate", "Good", "Very Good", "Excellent" } },
                        new() { Question = "What is the employee's potential for growth and increased responsibility?", LikertScale = new[] { "Limited", "Moderate", "Good", "High", "Exceptional" } }
                    }
                }
            },
            new()
            {
                IdSuffix = "tt-project-goal-setting",
                Name = "Goal Setting & Development Plan",
                Description = "Define SMART goals and a professional development plan for the upcoming review period.",
                TaskTypeId = "project-task",
                TypeSpecificData = new ProjectTaskTemplateTable
                {
                    Brief = "Collaboratively define 3-5 SMART (Specific, Measurable, Achievable, Relevant, Time-bound) goals for the upcoming review period, along with a professional development plan.",
                    Deliverable = "A documented set of agreed-upon goals and a development plan with milestones, signed off by both employee and manager.",
                    Objectives = new List<ProjectObjective>
                    {
                        new() { Id = "obj-draft-goals", Objective = "Draft 3-5 SMART goals for the upcoming period", Required = true },
                        new() { Id = "obj-align-priorities", Objective = "Align goals with team and organisational priorities", Required = true },
                        new() { Id = "obj-dev-plan", Objective = "Create a professional development plan with learning objectives", Required = true },
                        new() { Id = "obj-signoff", Objective = "Obtain mutual sign-off from employee and manager", Required = true }
                    },
                    Skills = new List<string> { "Goal Setting", "Career Development", "Coaching" },
                    SupportLinks = new List<ProjectSupportLink>
                    {
                        new() { Id = "link-smart-guide", Description = "Guide to writing SMART goals", LinkLabel = "SMART Goals Guide", Link = "" },
                        new() { Id = "link-learning-budget", Description = "Information about learning and development budget", LinkLabel = "L&D Budget", Link = "" }
                    }
                }
            }
        };

        // ── Seed Workflow Template Definitions ──────────────────────────────
        // Nodes reference task templates by their IdSuffix (resolved at seed time).

        private sealed class SeedWorkflowTemplateDef
        {
            public required string IdSuffix { get; init; }
            public required string Name { get; init; }
            public required string Description { get; init; }
            public required bool IsOnboardingWF { get; init; }
            public required SeedWorkflowNodeDef[] PreflowNodes { get; init; }
            public required SeedWorkflowNodeDef[] MainflowNodes { get; init; }
        }

        private sealed class SeedWorkflowNodeDef
        {
            public required string TaskTemplateIdSuffix { get; init; }
            public required int Order { get; init; }
            /// <summary>Assignee account ID suffix — resolved to the tenant admin at seed time.</summary>
            public string? AssigneeIdSuffix { get; init; }
            public int? DaysUntilDue { get; init; }
            public string[]? DependencyTaskTemplateIdSuffixes { get; init; }
        }

        private static readonly SeedWorkflowTemplateDef[] DefaultWorkflowTemplateDefs =
        {
            // ── Expense Report Approval ────────────────────────────────────
            new()
            {
                IdSuffix = "wf-expense-report",
                Name = "Expense Report Approval",
                Description = "Submit and approve an expense report: upload receipts, review policy, itemise expenses, and obtain manager approval.",
                IsOnboardingWF = false,
                PreflowNodes = Array.Empty<SeedWorkflowNodeDef>(),
                MainflowNodes = new[]
                {
                    new SeedWorkflowNodeDef { TaskTemplateIdSuffix = "tt-upload-receipts", Order = 1, DaysUntilDue = 3 },
                    new SeedWorkflowNodeDef { TaskTemplateIdSuffix = "tt-read-expense-policy", Order = 2, DaysUntilDue = 2 },
                    new SeedWorkflowNodeDef { TaskTemplateIdSuffix = "tt-checklist-expense-items", Order = 3, DaysUntilDue = 5 },
                    new SeedWorkflowNodeDef
                    {
                        TaskTemplateIdSuffix = "tt-project-manager-approval", Order = 4, AssigneeIdSuffix = "admin", DaysUntilDue = 5,
                        DependencyTaskTemplateIdSuffixes = new[] { "tt-upload-receipts", "tt-checklist-expense-items" }
                    }
                }
            },

            // ── Performance Review Cycle ───────────────────────────────────
            new()
            {
                IdSuffix = "wf-performance-review",
                Name = "Performance Review Cycle",
                Description = "Complete a full performance review cycle: read guidelines, self-assessment, manager assessment, and goal setting for the next period.",
                IsOnboardingWF = false,
                PreflowNodes = Array.Empty<SeedWorkflowNodeDef>(),
                MainflowNodes = new[]
                {
                    new SeedWorkflowNodeDef { TaskTemplateIdSuffix = "tt-read-perf-review-policy", Order = 1, DaysUntilDue = 3 },
                    new SeedWorkflowNodeDef
                    {
                        TaskTemplateIdSuffix = "tt-feedback-self-assessment", Order = 2, DaysUntilDue = 5,
                        DependencyTaskTemplateIdSuffixes = new[] { "tt-read-perf-review-policy" }
                    },
                    new SeedWorkflowNodeDef
                    {
                        TaskTemplateIdSuffix = "tt-feedback-manager-assessment", Order = 3, AssigneeIdSuffix = "admin", DaysUntilDue = 7,
                        DependencyTaskTemplateIdSuffixes = new[] { "tt-feedback-self-assessment" }
                    },
                    new SeedWorkflowNodeDef
                    {
                        TaskTemplateIdSuffix = "tt-project-goal-setting", Order = 4, DaysUntilDue = 10,
                        DependencyTaskTemplateIdSuffixes = new[] { "tt-feedback-manager-assessment" }
                    }
                }
            }
        };

        public static async Task InitializeAsync(IServiceProvider services, ILogger logger, CancellationToken cancellationToken = default)
        {
            const int maxAttempts = 10;
            var delay = TimeSpan.FromSeconds(3);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using var scope = services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Creates the database schema from the EF Core model when missing.
                    await context.Database.EnsureCreatedAsync(cancellationToken);
                    await EnsureOrganisationLogoColumnsAsync(context, cancellationToken);
                    await SeedBaselineDataAsync(context, cancellationToken);

                    logger.LogInformation("Database initialization completed.");
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning(ex, "Database initialization attempt {Attempt}/{MaxAttempts} failed. Retrying in {DelaySeconds}s.", attempt, maxAttempts, delay.TotalSeconds);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            throw new InvalidOperationException("Database initialization failed after all retry attempts.");
        }

        private static async Task EnsureOrganisationLogoColumnsAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            if (!await OrganisationColumnExistsAsync(context, "LogoImageData", cancellationToken))
            {
                const string addLogoDataColumnSql = "ALTER TABLE organisation ADD COLUMN LogoImageData MEDIUMBLOB NULL;";
                await RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(context.Database, addLogoDataColumnSql, cancellationToken);
            }

            if (!await OrganisationColumnExistsAsync(context, "LogoImageMimeType", cancellationToken))
            {
                const string addLogoMimeTypeColumnSql = "ALTER TABLE organisation ADD COLUMN LogoImageMimeType LONGTEXT NULL;";
                await RelationalDatabaseFacadeExtensions.ExecuteSqlRawAsync(context.Database, addLogoMimeTypeColumnSql, cancellationToken);
            }
        }

        private static async Task<bool> OrganisationColumnExistsAsync(ApplicationDbContext context, string columnName, CancellationToken cancellationToken)
        {
            var connectionString = context.Database.GetConnectionString();
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };
            var dbName = connectionStringBuilder["Database"]?.ToString();

            if (string.IsNullOrWhiteSpace(dbName) && connectionStringBuilder.ContainsKey("Initial Catalog"))
            {
                dbName = connectionStringBuilder["Initial Catalog"]?.ToString();
            }

            if (string.IsNullOrWhiteSpace(dbName))
            {
                throw new InvalidOperationException("Database name was not found in connection string while checking organisation columns.");
            }

            var columnLookupSql = @"
                SELECT COUNT(*) AS Value
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = {0}
                AND TABLE_NAME = 'organisation'
                AND COLUMN_NAME = {1}";

            var count = await context.Database.SqlQueryRaw<long>(columnLookupSql, dbName, columnName).SingleAsync(cancellationToken);
            return count > 0;
        }

        private static async Task SeedBaselineDataAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var hasChanges = false;
            var baseTierId = defaultSubscriptionTiers.First().Id;

            // Bypass tenant query filters during bootstrap so seed checks remain globally idempotent.
            var unfilteredTiers = context.SubscriptionTierEntitlements.IgnoreQueryFilters();
            var unfilteredTaskTypes = context.taskTypes.IgnoreQueryFilters();
            var unfilteredTenants = context.Tennants.IgnoreQueryFilters();
            var unfilteredOrganisations = context.Organisations.IgnoreQueryFilters();
            var unfilteredDepartments = context.Departments.IgnoreQueryFilters();
            var unfilteredAccounts = context.Accounts.IgnoreQueryFilters();

            // Subscription tier
            foreach (var tier in defaultSubscriptionTiers)
            {
                if (!await unfilteredTiers.AnyAsync(t => t.Id == tier.Id, cancellationToken))
                {
                    await context.SubscriptionTierEntitlements.AddAsync(tier, cancellationToken);
                    hasChanges = true;
                }
            }

            // Task types are globally keyed by Id, so seed them once.
            var sharedTaskTypeTenantId = DefaultTenantSeeds.First().TenantId;
            var existingTaskTypeIds = await unfilteredTaskTypes
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);
            var taskTypeIdSet = existingTaskTypeIds.ToHashSet(StringComparer.Ordinal);

            foreach (var (id, name) in DefaultTaskTypes)
            {
                if (taskTypeIdSet.Contains(id))
                {
                    continue;
                }

                await context.taskTypes.AddAsync(new TaskTypeTable
                {
                    Id = id,
                    TaskName = name,
                    TenantId = sharedTaskTypeTenantId
                }, cancellationToken);
                hasChanges = true;
            }

            foreach (var seed in DefaultTenantSeeds)
            {
                // Tenant
                if (!await unfilteredTenants.AnyAsync(t => t.Id == seed.TenantId, cancellationToken))
                {
                    await context.Tennants.AddAsync(new TenantTable
                    {
                        Id = seed.TenantId,
                        CreatedDateTime = DateTime.Now,
                        OwnerAccountId = seed.AdminAccountId,
                    }, cancellationToken);
                    hasChanges = true;
                }

                // Organisation
                if (!await unfilteredOrganisations.AnyAsync(o => o.Id == seed.OrganisationId && o.TenantId == seed.TenantId, cancellationToken))
                {
                    await context.Organisations.AddAsync(new OrganisationTable
                    {
                        Id = seed.OrganisationId,
                        Name = seed.OrganisationName,
                        TenantId = seed.TenantId
                    }, cancellationToken);
                    hasChanges = true;
                }

                // Department
                if (!await unfilteredDepartments.AnyAsync(d => d.Id == seed.DepartmentId && d.TenantId == seed.TenantId, cancellationToken))
                {
                    await context.Departments.AddAsync(new DepartmentTable
                    {
                        Id = seed.DepartmentId,
                        DisplayName = seed.DepartmentName,
                        TenantId = seed.TenantId,
                    }, cancellationToken);
                    hasChanges = true;
                }

                // Default admin account
                var adminEmail = seed.AdminEmail.ToLowerInvariant();
                if (!await unfilteredAccounts.AnyAsync(a => a.EmailAddress.ToLower() == adminEmail && a.TenantId == seed.TenantId, cancellationToken))
                {
                    await context.Accounts.AddAsync(new AccountTable
                    {
                        Id = seed.AdminAccountId,
                        DisplayName = seed.AdminDisplayName,
                        EmailAddress = seed.AdminEmail,
                        HashedPassword = ComputeSha256Hex(DefaultAdminPlainPassword),
                        IsSupervisor = true,
                        DepartmentId = seed.DepartmentId,
                        OrganisationId = seed.OrganisationId,
                        AccountStatus = RegisteredAccountStatus,
                        TenantId = seed.TenantId
                    }, cancellationToken);
                    hasChanges = true;
                }

                // Optional non-admin accounts
                if (seed.NonAdminAccounts != null)
                {
                    foreach (var nonAdmin in seed.NonAdminAccounts)
                    {
                        var nonAdminEmail = nonAdmin.Email.ToLowerInvariant();
                        if (!await unfilteredAccounts.AnyAsync(a => a.EmailAddress.ToLower() == nonAdminEmail && a.TenantId == seed.TenantId, cancellationToken))
                        {
                            await context.Accounts.AddAsync(new AccountTable
                            {
                                Id = nonAdmin.AccountId,
                                DisplayName = nonAdmin.DisplayName,
                                EmailAddress = nonAdmin.Email,
                                HashedPassword = ComputeSha256Hex(DefaultNonAdminPlainPassword),
                                IsSupervisor = false,
                                DepartmentId = seed.DepartmentId,
                                OrganisationId = seed.OrganisationId,
                                AccountStatus = RegisteredAccountStatus,
                                TenantId = seed.TenantId
                            }, cancellationToken);
                            hasChanges = true;
                        }
                    }
                }

                // Tenant subscription — tenant 1 gets tier 1, tenant 2 gets tier 2
                var subscriptionTierId = seed.TenantId == "default-tenant-1"
                    ? "tier-1-subscription"
                    : "tier-2-subscription";

                if (!await context.TenantSubscriptions.AnyAsync(ts => ts.TenantId == seed.TenantId, cancellationToken))
                {
                    await context.TenantSubscriptions.AddAsync(new TenantSubscriptionTable
                    {
                        Id = $"{seed.TenantId}-subscription",
                        TenantId = seed.TenantId,
                        SubscriptionTierId = subscriptionTierId,
                        CreatedDate = DateTime.Now,
                        StripeCustomerId = "",
                        StripeSubscriptionId = "",
                        StripeSubscriptionStatus = "active",
                        StripeCurrentPeriodEnd = DateTime.Now.AddMonths(12)
                    }, cancellationToken);
                    hasChanges = true;
                }
            }

            // ── Seed Task Templates & Workflow Templates per tenant ──────
            var unfilteredTaskTemplates = context.taskTemplates.IgnoreQueryFilters();
            var unfilteredWorkflowTemplates = context.workflowTemplates.IgnoreQueryFilters();

            foreach (var seed in DefaultTenantSeeds)
            {
                var tenantId = seed.TenantId;
                var adminId = seed.AdminAccountId;

                // Resolve task template ID : {tenantId}-{suffix}
                string TtId(string suffix) => $"{tenantId}-{suffix}";
                // Resolve workflow template ID
                string WfId(string suffix) => $"{tenantId}-{suffix}";
                // Resolve workflow node ID
                string WfnId(string wfSuffix, int order) => $"{tenantId}-{wfSuffix}-node-{order}";

                // ── 1. Task Templates ────────────────────────────────
                foreach (var def in DefaultTaskTemplateDefs)
                {
                    var ttId = TtId(def.IdSuffix);

                    if (await unfilteredTaskTemplates.AnyAsync(t => t.Id == ttId && t.TenantId == tenantId, cancellationToken))
                        continue;

                    var now = DateTime.UtcNow;
                    var taskTemplate = new TaskTemplateTable
                    {
                        Id = ttId,
                        Name = def.Name,
                        Description = def.Description,
                        CreatorAccountId = adminId,
                        DateCreated = now,
                        TaskTypeId = def.TaskTypeId,
                        Status = "active",
                        LastModifiedTimestamp = now,
                        TenantId = tenantId
                    };
                    await context.taskTemplates.AddAsync(taskTemplate, cancellationToken);
                    hasChanges = true;

                    // Insert type-specific template data — deep-clone the shared
                    // definition object so each tenant gets its own row.
                    var typeData = CloneTaskTypeData(def.TypeSpecificData);
                    typeData.Id = TtId($"{def.IdSuffix}-data");
                    typeData.TaskTemplateId = ttId;
                    typeData.TenantId = tenantId;

                    switch (typeData)
                    {
                        case ChecklistTaskTemplateTable ct:
                            await context.checklistTaskTemplates.AddAsync(ct, cancellationToken);
                            break;
                        case FileUploadTaskTemplateTable fut:
                            await context.fileUploadTaskTemplates.AddAsync(fut, cancellationToken);
                            break;
                        case ReadDocumentTaskTemplateTable rdt:
                            await context.readDocumentTaskTemplates.AddAsync(rdt, cancellationToken);
                            break;
                        case ProjectTaskTemplateTable pt:
                            await context.projectTaskTemplates.AddAsync(pt, cancellationToken);
                            break;
                        case FeedbackTaskTemplateTable ft:
                            await context.feedbackTaskTemplates.AddAsync(ft, cancellationToken);
                            break;
                    }
                }

                // ── 2. Workflow Templates + Nodes + Dependencies ──────
                foreach (var wfDef in DefaultWorkflowTemplateDefs)
                {
                    var wfId = WfId(wfDef.IdSuffix);

                    if (await unfilteredWorkflowTemplates.AnyAsync(w => w.Id == wfId && w.TenantId == tenantId, cancellationToken))
                        continue;

                    var workflowTemplate = new WorkflowTemplateTable
                    {
                        Id = wfId,
                        Name = wfDef.Name,
                        Description = wfDef.Description,
                        IsOnboardingWF = wfDef.IsOnboardingWF,
                        Status = "active",
                        TenantId = tenantId
                    };
                    await context.workflowTemplates.AddAsync(workflowTemplate, cancellationToken);
                    hasChanges = true;

                    // Track created nodes locally (by TaskTemplateId) so dependencies can be
                    // resolved in-memory — the DB hasn't been saved yet at this point.
                    var nodesByTaskTemplateId = new Dictionary<string, WorkflowTemplateNodeTable>(StringComparer.Ordinal);
                    var pendingDependencies = new List<(string nodeId, string[] depTaskTemplateIdSuffixes)>();

                    // Create all nodes first (preflow then mainflow)
                    foreach (var (nodeDefs, section) in new[]
                    {
                        (wfDef.PreflowNodes, "preflowtasks"),
                        (wfDef.MainflowNodes, "mainflowtasks")
                    })
                    {
                        foreach (var nodeDef in nodeDefs)
                        {
                            var nodeId = WfnId(wfDef.IdSuffix, nodeDef.Order);
                            var taskTemplateId = TtId(nodeDef.TaskTemplateIdSuffix);
                            var assigneeId = nodeDef.AssigneeIdSuffix == "admin" ? adminId : nodeDef.AssigneeIdSuffix;

                            var node = new WorkflowTemplateNodeTable
                            {
                                Id = nodeId,
                                WorkflowTemplateId = wfId,
                                TaskTemplateId = taskTemplateId,
                                AssigneeId = string.IsNullOrEmpty(assigneeId) ? adminId : assigneeId,
                                Order = nodeDef.Order,
                                WorkflowSection = section,
                                DaysUntilDue = nodeDef.DaysUntilDue,
                                TenantId = tenantId
                            };
                            await context.workflowTemplateNodes.AddAsync(node, cancellationToken);
                            nodesByTaskTemplateId[taskTemplateId] = node;

                            if (nodeDef.DependencyTaskTemplateIdSuffixes is { Length: > 0 })
                            {
                                pendingDependencies.Add((nodeId, nodeDef.DependencyTaskTemplateIdSuffixes));
                            }
                        }
                    }

                    // Now resolve all dependencies from the in-memory node map
                    foreach (var (nodeId, depSuffixes) in pendingDependencies)
                    {
                        foreach (var depSuffix in depSuffixes)
                        {
                            var depTaskTtId = TtId(depSuffix);
                            if (nodesByTaskTemplateId.TryGetValue(depTaskTtId, out var depNode))
                            {
                                var dep = new NodeTaskDependencyTable
                                {
                                    Id = $"{nodeId}-dep-{depNode.Id}",
                                    NodeId = nodeId,
                                    DependencyNodeId = depNode.Id,
                                    WorkflowTemplateId = wfId,
                                    TenantId = tenantId
                                };
                                await context.workflowTemplateNodeDependencies.AddAsync(dep, cancellationToken);
                            }
                        }
                    }
                }
            }

            if (hasChanges)
            {
                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private static dynamic CloneTaskTypeData(object source)
        {
            // Deep-clone via JSON so each tenant gets its own row
            // instead of mutating the shared static definition.
            var json = System.Text.Json.JsonSerializer.Serialize(source, source.GetType());
            return System.Text.Json.JsonSerializer.Deserialize(json, source.GetType())!;
        }

        private static string ComputeSha256Hex(string value)
        {
            using var hashAlgorithm = SHA256.Create();
            var bytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            var builder = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
            {
                builder.Append(b.ToString("X2"));
            }

            return builder.ToString();
        }

        private sealed class TenantSeedDefinition
        {
            public required string TenantId { get; init; }
            public required string OrganisationId { get; init; }
            public required string OrganisationName { get; init; }
            public required string DepartmentId { get; init; }
            public required string DepartmentName { get; init; }
            public required string AdminAccountId { get; init; }
            public required string AdminDisplayName { get; init; }
            public required string AdminEmail { get; init; }
            public IReadOnlyList<NonAdminSeedDefinition>? NonAdminAccounts { get; init; }
        }

        private sealed class NonAdminSeedDefinition
        {
            public required string AccountId { get; init; }
            public required string DisplayName { get; init; }
            public required string Email { get; init; }
        }
    }
}