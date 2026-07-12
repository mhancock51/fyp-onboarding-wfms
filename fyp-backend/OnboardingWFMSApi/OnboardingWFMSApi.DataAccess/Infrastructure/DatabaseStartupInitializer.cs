using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using System.Security.Cryptography;
using System.Text;

namespace OnboardingWFMSApi.DataAccess.Infrastructure
{
    public static class DatabaseStartupInitializer
    {
        private const string DefaultOrganisationId = "my-org";
        private const string DefaultOrganisationName = "My Org";
        private const string DefaultDepartmentId = "default-department";
        private const string DefaultDepartmentName = "General";
        private const string DefaultAdminAccountId = "default-supervisor-admin";
        private const string DefaultAdminDisplayName = "Default Supervisor";
        private const string DefaultAdminEmail = "admin@test.co.uk";
        private const string DefaultAdminPlainPassword = "pword123";
        private const string RegisteredAccountStatus = "registered";

        private static readonly SubscriptionTierEntitlementTable[] defaultSubscriptionTiers =
        {
            new SubscriptionTierEntitlementTable()
            {
                Id = "base-tier",
                DisplayName = "Base tier",
                CanUploadDocuments = true,
                MaxActiveWorkflowInstances = 99,
                MaxDocumentStorageSpaceInMb = 999999,
                MaxUsers = 999,
                CreatedDate = DateTime.Now,
                IsActive = true,
            },
        };

        private static readonly (string Id, string Name)[] DefaultTaskTypes =
        {
            ("checklist", "Checklist"),
            ("upload-document", "Upload Document"),
            ("read-document", "Read Document"),
            ("project-task", "Project Task"),
            ("feedback", "Feedback Task")
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

        private static async Task SeedBaselineDataAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var hasChanges = false;

            // seed subscription data
            await context.SubscriptionTierEntitlements.AddAsync(defaultSubscriptionTiers.First());

            // create test tenant
            var tenant = await context.Tennants.AddAsync(new TenantTable()
            {
                Id = "default-tenant",
                CreatedDateTime = DateTime.Now,
                OwnerEmailAddress = "admin@test.co.uk",
                HasActiveSubscription = true,
                IsOnHold = false,
                SubscriptionTeirId = defaultSubscriptionTiers.First().Id,
            });

            var organisation = await context.Organisations.AddAsync(new OrganisationTable
            {
                Id = DefaultOrganisationId,
                Name = DefaultOrganisationName,
                TenantId = tenant.Entity.Id
            }, cancellationToken);
            hasChanges = true;

            var existingTaskTypeIds = await context.taskTypes
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
                    TenantId = tenant.Entity.Id
                }, cancellationToken);
                hasChanges = true;
            }

            if (!await context.Departments.AnyAsync(d => d.Id == DefaultDepartmentId, cancellationToken))
            {
                await context.Departments.AddAsync(new DepartmentTable
                {
                    Id = DefaultDepartmentId,
                    DisplayName = DefaultDepartmentName,
                    TenantId = tenant.Entity.Id,
                }, cancellationToken);
                hasChanges = true;
            }

            if (!await context.Accounts.AnyAsync(a => a.EmailAddress.ToLower() == DefaultAdminEmail.ToLower(), cancellationToken))
            {
                await context.Accounts.AddAsync(new AccountTable
                {
                    Id = DefaultAdminAccountId,
                    DisplayName = DefaultAdminDisplayName,
                    EmailAddress = DefaultAdminEmail,
                    HashedPassword = ComputeSha256Hex(DefaultAdminPlainPassword),
                    IsSupervisor = true,
                    DepartmentId = DefaultDepartmentId,
                    OrganisationId = organisation.Entity.Id,
                    AccountStatus = RegisteredAccountStatus,
                    TenantId = tenant.Entity.Id
                }, cancellationToken);
                hasChanges = true;
            }

            if (hasChanges)
            {
                await context.SaveChangesAsync(cancellationToken);
            }
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
    }
}
