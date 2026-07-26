using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;
using Stripe;
using Stripe.Checkout;

namespace OnboardingWFMSApi.BusinessLogic.TenantLogic
{
    public interface ITenantOnboardingLogic
    {
        public Task<HTTPResponse<string, string>> InitialiseTenant(CreateTenantPayload payload);
        public Task<HTTPResponse<string, string>> ActivateTenantWithSubscription(string tenantId, string subscriptionTierId, Subscription subscription);
        public Task<HTTPResponse<string, string>> ExtendTenantSubscription(string tenantId, string subscriptionTierId, Invoice invoice);
        public Task<HTTPResponse<string, string>> ScheduleTenantSubscriptionCancellation(string tenantId);
        public Task<HTTPResponse<string, string>> CancelTenantSubscriptionImmediately(string tenantId);
    }

    public class TenantBoardingLogic : ITenantOnboardingLogic
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionTierRepository _subscriptionTierRepository;
        private readonly ITenantSubscriptionRepository _tenantSubscriptionRepository;
        private readonly ILogger<TenantBoardingLogic> _logger;

        public TenantBoardingLogic(ITenantRepository tenantRepository, ILogger<TenantBoardingLogic> logger, ISubscriptionTierRepository subscriptionTierRepository, ITenantSubscriptionRepository tenantSubscriptionRepository)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
            _subscriptionTierRepository = subscriptionTierRepository;
            _tenantSubscriptionRepository = tenantSubscriptionRepository;
        }

        /// <summary>
        /// Takes a stripe checkout session and creates an active subscription for the given tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="subscriptionTierId"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public async Task<HTTPResponse<string, string>> ActivateTenantWithSubscription(string tenantId, string subscriptionTierId, Subscription subscription)
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

            // check there isn't already an active subscription linked to this tenant
            var tenantSubscriptions = await _tenantSubscriptionRepository.GetTenantSubscriptionByTenantId(tenantId);
            if (tenantSubscriptions.Count() > 0)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    HttpCode = 400,
                    Error = "Tenant already has an active subscription"
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

            // todo: add validation that the invoice product matches product id of subscription tier

            try
            {
                // update status and creat subscription link 
                tenant.Status = TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS;
                await _tenantRepository.UpdateAsync(tenant);
                _logger.LogInformation($"Successfully moved tenant status to {TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS} and set subscription tier to {subscriptionTier.Id}");

                var tenantSubscription = new TenantSubscriptionTable()
                {
                    TenantId = tenantId,
                    SubscriptionTierId = subscriptionTierId,
                    CreatedDate = DateTime.Now,
                    StripeCustomerId = subscription.CustomerId,
                    StripeSubscriptionId = subscription.Id,
                    StripeCurrentPeriodEnd = subscription.Items.First().CurrentPeriodEnd,
                    StripeSubscriptionStatus = "active"
                };

                await _tenantSubscriptionRepository.AddAsync(tenantSubscription);
                
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

        public async Task<HTTPResponse<string, string>> ScheduleTenantSubscriptionCancellation(string tenantId)
        {
            // var result = new HTTPResponse<string, string>()
            // {
            //     Success = false,
            //     HttpCode = 500,
            //     Error = "Not implemented"
            // };

            // // get tenant, check if it exists
            // var tenant = await _tenantRepository.GetById(tenantId);
            // if (tenant == null)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant doesn't exist",
            //         HttpCode = 400
            //     };
            // }

            // if (tenant.Status != TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant isn't active",
            //         HttpCode = 400
            //     };
            // }

            // // find their subscription
            // var subscriptions = await _tenantSubscriptionRepository.GetTenantSubscriptionByTenantId(tenantId);
            // if (subscriptions.Count() == 0)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant doesn't have a subscription",
            //         HttpCode = 500
            //     };
            // }

            // var subscription = subscriptions.First();
            // if (!subscription.IsActive)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant subscription is inactive",
            //         HttpCode = 400
            //     };    
            // }

            // if (subscription.CancelAtEndOfPeriod)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant is already scheduled to be cancelled",
            //         HttpCode = 400
            //     };
            // }

            try
            {
                // set cancel at end of period to true
                // subscription.CancelAtEndOfPeriod = true;
                // await _tenantSubscriptionRepository.UpdateAsync(subscription);

                _logger.LogInformation("Successfully scheduled subscription to end at end of current period");
                return new HTTPResponse<string, string>()
                {
                    Success = true,
                    Message = "Successfully scheduled subscription to end at end of current period",
                    HttpCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to schedule subscription to be cancelled: {ex}");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Message = "Failed to schedule subscription to be cancelled",
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
                });
                return new HTTPResponse<string, string>() { Success = true, Message = "Successfully created tenant", HttpCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create tenant: {ex}");
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to create tenant", HttpCode = 500 };
            }
        }

        public async Task<HTTPResponse<string, string>> CancelTenantSubscriptionImmediately(string tenantId)
        {
            var result = new HTTPResponse<string, string>()
            {
                Success = false,
                HttpCode = 500,
                Error = "Not implemented"
            };

            // get tenant, check if it exists
            var tenant = await _tenantRepository.GetById(tenantId);
            if (tenant == null)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant doesn't exist",
                    HttpCode = 400
                };
            }

            if (tenant.Status != TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant isn't active",
                    HttpCode = 400
                };
            }

            // find their subscription
            var subscriptions = await _tenantSubscriptionRepository.GetTenantSubscriptionByTenantId(tenantId);
            if (subscriptions.Count() == 0)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant doesn't have a subscription",
                    HttpCode = 500
                };
            }

            // var subscription = subscriptions.First();
            // if (!subscription.IsActive)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant subscription is inactive",
            //         HttpCode = 400
            //     };    
            // }

            // if (subscription.CancelAtEndOfPeriod)
            // {
            //     return new HTTPResponse<string, string>()
            //     {
            //         Success = false,
            //         Error = "Tenant is already scheduled to be cancelled",
            //         HttpCode = 400
            //     };
            // }

            try
            {
                // set cancel at end of period to true
                // subscription.IsActive = false;
                //await _tenantSubscriptionRepository.UpdateAsync(subscription);

                _logger.LogInformation("Successfully marked subscription as inactive");
                return new HTTPResponse<string, string>()
                {
                    Success = true,
                    Message = "Successfully cancelled subscription with immediate effect",
                    HttpCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to schedule subscription to be cancelled: {ex}");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Message = "Failed to schedule subscription to be cancelled",
                    HttpCode = 500
                };
            }
        }

        public async Task<HTTPResponse<string, string>> ExtendTenantSubscription(string tenantId, string subscriptionTierId, Invoice invoice)
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

            if (tenant.Status != TenantTable.TENANT_ACTIVE_SUBSCRIPTION_STATUS)
            {
                _logger.LogError($"Tenant must have an active subscription status, current status: ({tenant.Status})");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant must have an active subscription status",
                    HttpCode = 400
                };
            }

            try
            {
                // find the tenant subscription
                var tenantSubscriptions = await _tenantSubscriptionRepository.GetTenantSubscriptionByTenantId(tenantId);
                if (tenantSubscriptions == null || tenantSubscriptions.Count(s => s.StripeSubscriptionStatus == "active") == 0)
                {
                    throw new Exception("Tenant subscription doesn't exist");
                }
                var subscription = tenantSubscriptions.First(s => s.StripeSubscriptionStatus == "active");
                var previousCurrentPeriodEnd = subscription.StripeCurrentPeriodEnd;
                
                subscription.StripeCurrentPeriodEnd = invoice.Lines.First().Period.End;
                subscription.StripeSubscriptionStatus = "active";
                _logger.LogInformation($"Subscription extended from {previousCurrentPeriodEnd} to {subscription.StripeCurrentPeriodEnd} for customer: {subscription.StripeCustomerId} (sub: {subscription.StripeSubscriptionId})");
                return new HTTPResponse<string, string>()
                {
                    Success = true,
                    Message = "Successfully extended subscription",
                    HttpCode = 200
                };
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error extending tenant subscription: {ex}");
                return new HTTPResponse<string, string>() {
                    Success = false,
                    Error = "Failed to extend tenant subscription",
                    HttpCode = 500
                };
            }
        }
    }
}