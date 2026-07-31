using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;

namespace OnboardingWFMSApi.BusinessLogic.TenantManagement
{
    public interface ITenantEntitlementLogic
    {
        public Task<HTTPResponse<string, string>> CanCreateWorkflowInstance(string tenantId);
        public Task<HTTPResponse<string, string>> CanUploadDocument(string tenantId);
        public Task<HTTPResponse<string, string>> CanInviteUser();
    }

    public class TenantEntitlementLogic : ITenantEntitlementLogic
    {
        private readonly ITenantSubscriptionRepository _tenantSubscriptions;
        private readonly ISubscriptionTierRepository _subscriptionTiers; 
        private readonly IWorkflowInstanceRepository _workflowInstances;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<TenantEntitlementLogic> _logger;
        private readonly ICurrentTenantService _currentTenantService;

        public TenantEntitlementLogic(ITenantSubscriptionRepository tenantSubscriptions, ILogger<TenantEntitlementLogic> logger, ISubscriptionTierRepository subscriptionTiers, IWorkflowInstanceRepository workflowInstances, IAccountRepository accountRepository, ICurrentTenantService currentTenantService)
        {
            _tenantSubscriptions = tenantSubscriptions;
            _logger = logger;
            _subscriptionTiers = subscriptionTiers;
            _workflowInstances = workflowInstances;
            _accountRepository = accountRepository;
            _currentTenantService = currentTenantService;
        }

        private async Task<HTTPResponse<SubscriptionTierEntitlementTable, string>> GetSubscriptionTier(string tenantId)
        {
            var tenantSubscription = await _tenantSubscriptions.GetTenantSubscriptionByTenantId(tenantId);
            if (tenantSubscription == null) 
                return new HTTPResponse<SubscriptionTierEntitlementTable, string>() {Success = false, Error = "Tenant subscription doesn't exist", HttpCode = 400 };

            var subscriptionTier = await _subscriptionTiers.GetById(tenantSubscription.SubscriptionTierId);
            if (subscriptionTier == null)
                return new HTTPResponse<SubscriptionTierEntitlementTable, string>() {Success = false, Error = "Subscription tier doesn't exist", HttpCode = 500 };

            return new HTTPResponse<SubscriptionTierEntitlementTable, string>()
            {
                Success = true,
                Data = subscriptionTier,
                HttpCode = 200
            };
        }

        public async Task<HTTPResponse<string, string>> CanCreateWorkflowInstance(string tenantId)
        {
            _logger.LogDebug("Handling requirement...");
            var result = await GetSubscriptionTier(tenantId);
            if (!result.Success || !result.HasData)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = result.Error,
                    HttpCode = result.HttpCode
                };
            }
            var subscriptionTier = result.Data;

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

        public async Task<HTTPResponse<string, string>> CanInviteUser()
        {
            _logger.LogDebug("Handling requirement...");
            var result = await GetSubscriptionTier(_currentTenantService.TenantId);
            if (!result.Success || !result.HasData)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = result.Error,
                    HttpCode = result.HttpCode
                };
            }
            var subscriptionTier = result.Data;

            var numberOfAccounts = await _accountRepository.GetNumberOfAccounts();
            if (numberOfAccounts >= subscriptionTier.MaxUsers)
            {
                _logger.LogDebug("Max users reached");
                return new HTTPResponse<string, string>() {Success = false, Error = "Max users reached", HttpCode = 400};
            }
            else
            {
                _logger.LogDebug("Max users limit not reached yet");
                return new HTTPResponse<string, string>() {Success = true, HttpCode = 200};
            }
        }

        public async Task<HTTPResponse<string, string>> CanUploadDocument(string tenantId)
        {

            throw new NotImplementedException();
        }
    }
}