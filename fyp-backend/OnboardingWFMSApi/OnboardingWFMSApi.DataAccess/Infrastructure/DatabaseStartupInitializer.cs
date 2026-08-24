using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;

namespace OnboardingWFMSApi.DataAccess.Infrastructure
{
    /// <summary>
    /// Creates the database schema and populates baseline seed data
    /// from <see cref="BaselineSeedData"/> on first run.
    /// </summary>
    public static class DatabaseStartupInitializer
    {
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
            var baseTierId = BaselineSeedData.DefaultSubscriptionTiers.First().Id;

            // Bypass tenant query filters during bootstrap so seed checks remain globally idempotent.
            var unfilteredTiers = context.SubscriptionTierEntitlements.IgnoreQueryFilters();
            var unfilteredTaskTypes = context.taskTypes.IgnoreQueryFilters();
            var unfilteredTenants = context.Tennants.IgnoreQueryFilters();
            var unfilteredOrganisations = context.Organisations.IgnoreQueryFilters();
            var unfilteredDepartments = context.Departments.IgnoreQueryFilters();
            var unfilteredAccounts = context.Accounts.IgnoreQueryFilters();

            // Subscription tier
            foreach (var tier in BaselineSeedData.DefaultSubscriptionTiers)
            {
                if (!await unfilteredTiers.AnyAsync(t => t.Id == tier.Id, cancellationToken))
                {
                    await context.SubscriptionTierEntitlements.AddAsync(tier, cancellationToken);
                    hasChanges = true;
                }
            }

            // Task types are globally keyed by Id, so seed them once.
            var sharedTaskTypeTenantId = BaselineSeedData.DefaultTenantSeeds.First().TenantId;
            var existingTaskTypeIds = await unfilteredTaskTypes
                .Select(t => t.Id)
                .ToListAsync(cancellationToken);
            var taskTypeIdSet = existingTaskTypeIds.ToHashSet(StringComparer.Ordinal);

            foreach (var (id, name) in BaselineSeedData.DefaultTaskTypes)
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

            foreach (var seed in BaselineSeedData.DefaultTenantSeeds)
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
                        HashedPassword = ComputeSha256Hex(BaselineSeedData.DefaultAdminPlainPassword),
                        IsSupervisor = true,
                        DepartmentId = seed.DepartmentId,
                        OrganisationId = seed.OrganisationId,
                        AccountStatus = BaselineSeedData.RegisteredAccountStatus,
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
                                HashedPassword = ComputeSha256Hex(BaselineSeedData.DefaultNonAdminPlainPassword),
                                IsSupervisor = false,
                                DepartmentId = seed.DepartmentId,
                                OrganisationId = seed.OrganisationId,
                                AccountStatus = BaselineSeedData.RegisteredAccountStatus,
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

            foreach (var seed in BaselineSeedData.DefaultTenantSeeds)
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
                foreach (var def in BaselineSeedData.DefaultTaskTemplateDefs)
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
                    var typeData = BaselineSeedData.CloneTaskTypeData(def.TypeSpecificData);
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
                foreach (var wfDef in BaselineSeedData.DefaultWorkflowTemplateDefs)
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