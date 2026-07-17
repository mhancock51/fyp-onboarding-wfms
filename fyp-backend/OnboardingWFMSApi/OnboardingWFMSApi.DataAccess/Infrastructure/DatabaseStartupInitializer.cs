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
        private const string DefaultAdminPlainPassword = "pword123";
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
            },
        };

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
            var baseTierId = defaultSubscriptionTiers.First().Id;

            // Bypass tenant query filters during bootstrap so seed checks remain globally idempotent.
            var unfilteredTiers = context.SubscriptionTierEntitlements.IgnoreQueryFilters();
            var unfilteredTaskTypes = context.taskTypes.IgnoreQueryFilters();
            var unfilteredTenants = context.Tennants.IgnoreQueryFilters();
            var unfilteredOrganisations = context.Organisations.IgnoreQueryFilters();
            var unfilteredDepartments = context.Departments.IgnoreQueryFilters();
            var unfilteredAccounts = context.Accounts.IgnoreQueryFilters();

            // Subscription tier
            if (!await unfilteredTiers.AnyAsync(t => t.Id == baseTierId, cancellationToken))
            {
                await context.SubscriptionTierEntitlements.AddAsync(defaultSubscriptionTiers.First(), cancellationToken);
                hasChanges = true;
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
                        OwnerEmailAddress = seed.AdminEmail,
                        HasActiveSubscription = true,
                        IsOnHold = false,
                        SubscriptionTeirId = baseTierId,
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
        }
    }
}
