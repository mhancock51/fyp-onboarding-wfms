using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
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