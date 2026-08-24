using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using OnboardingWFMSApi.DataModels.Tables.Workflows;

namespace OnboardingWFMSApi.DataAccess.Infrastructure
{
    /// <summary>
    /// Static baseline seed definitions for populating a fresh database.
    /// Referenced by <see cref="DatabaseStartupInitializer"/>.
    /// </summary>
    public static class BaselineSeedData
    {
        public const string DefaultAdminPlainPassword = "pword123";
        public const string DefaultNonAdminPlainPassword = "pword123";
        public const string RegisteredAccountStatus = "registered";

        public static readonly TenantSeedDefinition[] DefaultTenantSeeds =
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

        public static readonly SubscriptionTierEntitlementTable[] DefaultSubscriptionTiers =
        {
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
                PriceId = "price_1U5lQkF3XLGavbWVubJCZ4Ap",
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
                PriceId = "price_1U5lQOF3XLGavbWVzZG3Cp28",
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

        public static readonly (string Id, string Name)[] DefaultTaskTypes =
        {
            ("checklist", "Checklist"),
            ("upload-document", "Upload Document"),
            ("read-document", "Read Document"),
            ("project-task", "Project Task"),
            ("feedback", "Feedback Task")
        };

        // ── Seed Task Template Definitions ──────────────────────────────────

        public static readonly SeedTaskTemplateDef[] DefaultTaskTemplateDefs =
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

        public static readonly SeedWorkflowTemplateDef[] DefaultWorkflowTemplateDefs =
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

        /// <summary>Deep-clone via JSON so each tenant gets its own row.</summary>
        public static dynamic CloneTaskTypeData(object source)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(source, source.GetType());
            return System.Text.Json.JsonSerializer.Deserialize(json, source.GetType())!;
        }
    }

    // ── Seed Definition Types ──────────────────────────────────────────────

    public sealed class SeedTaskTemplateDef
    {
        public required string IdSuffix { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required string TaskTypeId { get; init; }
        public required object TypeSpecificData { get; init; }
    }

    public sealed class SeedWorkflowTemplateDef
    {
        public required string IdSuffix { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required bool IsOnboardingWF { get; init; }
        public required SeedWorkflowNodeDef[] PreflowNodes { get; init; }
        public required SeedWorkflowNodeDef[] MainflowNodes { get; init; }
    }

    public sealed class SeedWorkflowNodeDef
    {
        public required string TaskTemplateIdSuffix { get; init; }
        public required int Order { get; init; }
        public string? AssigneeIdSuffix { get; init; }
        public int? DaysUntilDue { get; init; }
        public string[]? DependencyTaskTemplateIdSuffixes { get; init; }
    }

    public sealed class TenantSeedDefinition
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

    public sealed class NonAdminSeedDefinition
    {
        public required string AccountId { get; init; }
        public required string DisplayName { get; init; }
        public required string Email { get; init; }
    }
}
