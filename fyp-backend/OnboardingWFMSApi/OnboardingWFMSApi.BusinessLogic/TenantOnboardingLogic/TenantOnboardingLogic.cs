using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnboardingWFMSApi.BusinessLogic.StripeLogic;
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
        public Task<HTTPResponse<string, string>> InitialiseTenant(string ownerAccountId);
        public Task<HTTPResponse<string, string>> ActivateTenantWithSubscription(Subscription subscription);
        public Task<HTTPResponse<string, string>> ExtendTenantSubscription(Invoice invoice);
        public Task<HTTPResponse<string, string>> ChangeTenantSubscriptionStatusToOverdue(string stripeSubscriptionId, string stripeCustomerId);
        public Task<HTTPResponse<string, string>> DeleteTenantSubscription(string stripeSubscriptionId, string stripeCustomerId);

        public Task<bool> DoesTenantHaveSubscription(string tenantId);
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
        public async Task<HTTPResponse<string, string>> ActivateTenantWithSubscription(Subscription subscription)
        {
            // get meta data:
            subscription.Metadata.TryGetValue(StripeConstants.TENANT_ID_META_DATA_KEY, out string tenantId);
            subscription.Metadata.TryGetValue(StripeConstants.SUBSCRIPTION_TIER_ID_META_DATA_KEY, out string subscriptionTierId);
            if (string.IsNullOrEmpty(tenantId))
            {
                _logger.LogError("Tenant ID from meta data of checkout completed event can't be empty.");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Tenant ID from meta data of checkout completed event can't be empty.",
                    HttpCode = 400
                };
            }
            if (string.IsNullOrEmpty(subscriptionTierId))
            {
                _logger.LogError("Subscription tier ID from meta data of checkout completed event can't be empty.");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Subscription tier ID from meta data of checkout completed event can't be empty.",
                    HttpCode = 400
                };
            }

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

            // check there isn't already an active subscription linked to this tenant
            var existingSubscription = await _tenantSubscriptionRepository.GetTenantSubscriptionByTenantId(tenantId);            
            if (existingSubscription == null)
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
                await _tenantRepository.UpdateAsync(tenant);
                _logger.LogInformation($"Successfully activated subscription for tenant: {tenant.Id}");

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

        /// <summary>
        /// responsible for initial setup of tenant and creation of organisation, doesn't activate the tenant so access can't be made until subscription is called
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<HTTPResponse<string, string>> InitialiseTenant(string ownerAccountId)
        {
            try
            {
                await _tenantRepository.AddAsync(new TenantTable
                {
                    CreatedDateTime = DateTime.Now,          
                    OwnerAccountId = ownerAccountId      
                });
                return new HTTPResponse<string, string>() { Success = true, Message = "Successfully created tenant", HttpCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create tenant: {ex}");
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to create tenant", HttpCode = 500 };
            }
        }

        public async Task<HTTPResponse<string, string>> ExtendTenantSubscription(Invoice invoice)
        {
            if (invoice.Lines.Count() == 0)
            {
                throw new Exception("Invoice has no line items");
            }

            var subscriptionLine = invoice.Lines.First();

            // find subscription by stripe subscriptionId and customerId
            var subscription = await _tenantSubscriptionRepository.FindAsync(ts => ts.StripeSubscriptionId == subscriptionLine.SubscriptionId && ts.StripeCustomerId == invoice.CustomerId);
            if (subscription == null)
            {
                throw new Exception("Tenant subscription doesn't exist");                
            }
                        
            var previousCurrentPeriodEnd = subscription.StripeCurrentPeriodEnd;
            
            subscription.StripeCurrentPeriodEnd = invoice.Lines.First().Period.End;
            subscription.StripeSubscriptionStatus = "active";
            await _tenantSubscriptionRepository.UpdateAsync(subscription);

            _logger.LogInformation($"Subscription extended from {previousCurrentPeriodEnd} to {subscription.StripeCurrentPeriodEnd} for customer: {subscription.StripeCustomerId} (sub: {subscription.StripeSubscriptionId})");
            return new HTTPResponse<string, string>()
            {
                Success = true,
                Message = "Successfully extended subscription",
                HttpCode = 200
            };
        }

        public async Task<HTTPResponse<string, string>> ChangeTenantSubscriptionStatusToOverdue(string stripeSubscriptionId, string stripeCustomerId)
        {
            // find subscription by stripe subscriptionId and customerId            
            var subscription = await _tenantSubscriptionRepository.FindAsync(ts => ts.StripeSubscriptionId == stripeSubscriptionId && ts.StripeCustomerId == stripeCustomerId);
            if (subscription == null)
            {
                throw new Exception("Tenant subscription doesn't exist");                
            }
            
            subscription.StripeSubscriptionStatus = "overdue";
            await _tenantSubscriptionRepository.UpdateAsync(subscription);

            _logger.LogInformation($"Successfully moved tenant subscription to overdue");
            return new HTTPResponse<string, string>()
            {
                Success = true,
                Message = "Successfully extended subscription",
                HttpCode = 200
            };
        }

        public async Task<bool> DoesTenantHaveSubscription(string tenantId)
        {
            var tenantSubscription = await _tenantSubscriptionRepository.GetTenantSubscriptionByTenantId(tenantId);
            return tenantSubscription == null;
        }

        public async Task<HTTPResponse<string, string>> DeleteTenantSubscription(string stripeSubscriptionId, string stripeCustomerId)
        {
            try
            {
                var tenantSubscription = await _tenantSubscriptionRepository.FindAsync(ts => ts.StripeSubscriptionId == stripeSubscriptionId && ts.StripeCustomerId == stripeCustomerId);
                if (tenantSubscription == null)
                {
                    throw new Exception("Tenant subscription");
                }
                await _tenantSubscriptionRepository.DeleteAsync(tenantSubscription);
                return new HTTPResponse<string, string>()
                {
                    Success = true,
                    Message = "Successfully delete tenant subscription",
                    HttpCode = 200
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to delete tenant subscription: {ex}");
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Failed to delete tenant subscription",
                    HttpCode = 500
                };
            }
        }
    }
}