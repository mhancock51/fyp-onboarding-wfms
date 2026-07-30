using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;

namespace OnboardingWFMSApi.BusinessLogic.TenantManagement
{
    public interface ITenantEntitlementLogic
    {
        public Task<HTTPResponse<string, string>> CanCreateWorkflowInstance(string tenantId);
    }

    public class TenantEntitlementLogic : ITenantEntitlementLogic
    {
        private readonly ITenantSubscriptionRepository _tenantSubscriptions;
        private readonly ISubscriptionTierRepository _subscriptionTiers; 
        private readonly IWorkflowInstanceRepository _workflowInstances;
        private readonly ILogger<TenantEntitlementLogic> _logger;

        public TenantEntitlementLogic(ITenantSubscriptionRepository tenantSubscriptions, ILogger<TenantEntitlementLogic> logger, ISubscriptionTierRepository subscriptionTiers, IWorkflowInstanceRepository workflowInstances)
        {
            _tenantSubscriptions = tenantSubscriptions;
            _logger = logger;
            _subscriptionTiers = subscriptionTiers;
            _workflowInstances = workflowInstances;
        }

        public async Task<HTTPResponse<string, string>> CanCreateWorkflowInstance(string tenantId)
        {
            _logger.LogDebug("Handling requirement...");
            var tenantSubscription = await _tenantSubscriptions.GetTenantSubscriptionByTenantId(tenantId);
            if (tenantSubscription == null) 
                return new HTTPResponse<string, string>() {Success = false, Error = "Tenant subscription doesn't exist", HttpCode = 400 };

            var subscriptionTier = await _subscriptionTiers.GetById(tenantSubscription.SubscriptionTierId);
            if (subscriptionTier == null)
                return new HTTPResponse<string, string>() {Success = false, Error = "Subscription tier doesn't exist", HttpCode = 500 };


            int maxWorkflowInstances = subscriptionTier.MaxActiveWorkflowInstances;
            if (await _workflowInstances.CountActiveWorkflowInstances() >= maxWorkflowInstances)
            {
                _logger.LogDebug("Max workflow instances reached");
                return new HTTPResponse<string, string>() {Success = false, Error = "Max workflow instances reached", HttpCode = 400};
            }
            else
            {
                _logger.LogDebug("Max workflow instances limit not reached yet");
                return new HTTPResponse<string, string>() {Success = true, HttpCode = 200};
            }
        }
    }
}