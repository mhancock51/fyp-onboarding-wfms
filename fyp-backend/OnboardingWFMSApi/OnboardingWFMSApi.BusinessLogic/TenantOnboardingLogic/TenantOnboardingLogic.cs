using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;

namespace OnboardingWFMSApi.BusinessLogic.TenantLogic
{
    public interface ITenantOnboardingLogic
    {
        public Task<HTTPResponse<string, string>> InitialiseTenant(CreateTenantPayload payload);
        public Task<HTTPResponse<string, string>> ActivateTenantWithSubscription(string tenantId, string subscriptionTierId);
    }

    public class TenantBoardingLogic : ITenantOnboardingLogic
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionTierRepository _subscriptionTierRepository;
        private readonly ILogger<TenantBoardingLogic> _logger;

        public TenantBoardingLogic(ITenantRepository tenantRepository, ILogger<TenantBoardingLogic> logger, ISubscriptionTierRepository subscriptionTierRepository)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
            _subscriptionTierRepository = subscriptionTierRepository;
        }

        public async Task<HTTPResponse<string, string>> ActivateTenantWithSubscription(string tenantId, string subscriptionTierId)
        {
            var tenant = await _tenantRepository.GetById(tenantId);
            if (tenant == null)
            {
                _logger.LogError("Tenant doesn't exist");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant doesn't exist",
                    HttpCode = 400
                };
            }

            // check its status is "procurred"
            if (tenant.Status != TenantTable.TENANT_PROCURED_STATUS)
            {
                _logger.LogError($"Tenant has already exited the 'procured' state ({tenant.Status})");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant has already exited the 'procured' state",
                    HttpCode = 400
                };
            }

            // check tenant doesn't already have a subscription tier assigned
            if (!string.IsNullOrEmpty(tenant.SubscriptionTeirId))
            {
                _logger.LogError($"Tenant already has a subscription tier assigned");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant already has a subscription tier assigned",
                    HttpCode = 500
                };
            }

            // check subscription tier is active
            var subscriptionTier = await _subscriptionTierRepository.GetById(subscriptionTierId);
            if (subscriptionTier == null)
            {
                _logger.LogError($"Subscription tier doesn't exist ({subscriptionTierId})");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Subscription tier doesn't exist",
                    HttpCode = 500
                };
            }
            if (!subscriptionTier.IsActive)
            {
                _logger.LogError($"Subscription tier isn't active ({subscriptionTier.Id})");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Subscription tier isn't active",
                    HttpCode = 500
                };
            }
            // assign the new subscription tier and 
            try
            {
                tenant.SubscriptionTeirId = subscriptionTier.Id;
                tenant.Status = TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS;
                await _tenantRepository.UpdateAsync(tenant);
                _logger.LogInformation($"Successfully moved tenant status to {TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS} and set subscription tier to {subscriptionTier.Id}");
                return new HTTPResponse<string, string>()
                {
                    Success = true,
                    HttpCode = 200,
                    Message = "Successfully activated subscription"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update tenant: {ex}");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Failed to update tenant",
                    HttpCode = 500
                };
            }
        }

        /// <summary>
        /// responsible for initial setup of tenant and creation of organisation, doesn't activate the tenant so access can't be made until subscription is called
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<HTTPResponse<string, string>> InitialiseTenant(CreateTenantPayload payload)
        {
            try
            {
                await _tenantRepository.AddAsync(new TenantTable
                {
                    CreatedDateTime = DateTime.Now,
                    OwnerEmailAddress = payload.OwnerEmailAddress,
                    Status = TenantTable.TENANT_PROCURED_STATUS,
                    SubscriptionTeirId = payload.SubscriptionTeirId
                });
                return new HTTPResponse<string, string>() { Success = true, Message = "Successfully created tenant", HttpCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create tenant: {ex}");
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to create tenant", HttpCode = 500 };
            }
        }
    }
}