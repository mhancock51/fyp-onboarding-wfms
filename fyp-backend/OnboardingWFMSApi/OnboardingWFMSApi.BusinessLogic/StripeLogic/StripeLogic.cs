using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnboardingWFMSApi.BusinessLogic.TenantLogic;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables;
using Stripe;
using Stripe.Checkout;

namespace OnboardingWFMSApi.BusinessLogic.StripeLogic
{
    public interface IStripeLogic
    {
        public Task<HTTPResponse<Session, string>> CreateCheckoutSession(string tenantId, string tierId);
        public Task<HTTPResponse<string, string>> HandleStripeEvent(Event stripeEvent); 
    }

    public class StripeLogic : IStripeLogic
    {
        private readonly ILogger<StripeLogic> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionTierRepository _subscriptionTierRepository;
        private readonly string _stripeSecretKey;
        private readonly string _stripeMode;
        private readonly string _checkoutSuccessUrl;
        private readonly string _checkoutCancelUrl;

        private const string TENANT_ID_META_DATA_KEY = "TenantId";
        private const string SUBSCRIPTION_TIER_ID_META_DATA_KEY = "SubscriptionTierId";

        private readonly ITenantOnboardingLogic _tenantOnboardingLogic;

        public StripeLogic(
            ILogger<StripeLogic> logger,
            ITenantRepository tenantRepository,
            ISubscriptionTierRepository subscriptionTierRepository,
            IConfiguration configuration,
            ITenantOnboardingLogic tenantOnboardingLogic)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _subscriptionTierRepository = subscriptionTierRepository;
            _stripeSecretKey = configuration["Stripe:SecretKey"] ?? string.Empty;
            _stripeMode = (configuration["Stripe:Mode"] ?? "test").Trim().ToLowerInvariant();
            _checkoutSuccessUrl = configuration["Stripe:SuccessUrl"] ?? string.Empty;
            _checkoutCancelUrl = configuration["Stripe:CancelUrl"] ?? string.Empty;
            _tenantOnboardingLogic = tenantOnboardingLogic;
        }

        public async Task<HTTPResponse<Session, string>> CreateCheckoutSession(string tenantId, string tierId)
        {
            if (string.IsNullOrWhiteSpace(_stripeSecretKey))
            {
                _logger.LogError("Stripe secret key is missing. Set Stripe:SecretKey in configuration or Stripe__SecretKey as an environment variable.");
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 500,
                    Error = "Stripe is not configured."
                };
            }

            if (_stripeMode == "test" && _stripeSecretKey.StartsWith("sk_live_", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Stripe is configured for test mode but a live secret key was provided.");
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 500,
                    Error = "Stripe test mode is enabled, but a live API key is configured."
                };
            }

            if (_stripeMode == "live" && _stripeSecretKey.StartsWith("sk_test_", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Stripe is configured for live mode but a test secret key was provided.");
            }

            if (!Uri.TryCreate(_checkoutSuccessUrl, UriKind.Absolute, out _)
                || !Uri.TryCreate(_checkoutCancelUrl, UriKind.Absolute, out _))
            {
                _logger.LogError("Stripe checkout URLs are invalid. Configure absolute Stripe:SuccessUrl and Stripe:CancelUrl values.");
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 500,
                    Error = "Stripe checkout URLs are not configured correctly."
                };
            }

            // check tenant exists
            var tenant = await _tenantRepository.GetById(tenantId);
            if (tenant == null)
            {
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 400,
                    Message = "Tenant doesn't exist"
                };
            }
            // check tentant doesn't already have a subscription
            if (tenant.Status != TenantTable.TENANT_PROCURED_STATUS)
            {
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 400,
                    Message = "Can't create subscription for this tenant"
                };
            }

            // check that tier exists
            var subscriptionTier = await _subscriptionTierRepository.GetById(tierId);
            if (subscriptionTier == null)
            {
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 400,
                    Message = "Subscription tier doesn't exist"
                };
            }
            if (!subscriptionTier.IsActive)
            {
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 400,
                    Message = "Subscription tier isn't active"
                };
            }

            // create checkout
            try
            {
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            Price = subscriptionTier.PriceId,
                            Quantity = 1,
                        },
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        {
                            TENANT_ID_META_DATA_KEY, tenantId
                        },
                        {
                            SUBSCRIPTION_TIER_ID_META_DATA_KEY, subscriptionTier.Id
                        }
                    },
                    Mode = "subscription", 
                    SuccessUrl = _checkoutSuccessUrl,
                    CancelUrl = _checkoutCancelUrl,
                };

                var client = new StripeClient(_stripeSecretKey);
                var service = new SessionService(client);
                Session session = await service.CreateAsync(options);

                _logger.LogInformation($"Successfully created checkout for tenant: {tenant.Id}, owned by: {tenant.OwnerEmailAddress}");

                // Return the checkout URL to your frontend
                return new HTTPResponse<Session, string>()
                {
                    Success = true,
                    HttpCode = 200,
                    Data = session
                };
            }
            catch (StripeException e)
            {
                _logger.LogError($"Failed to create stripe checkout: {e.Message}");
                // Log error internally
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    HttpCode = 500,
                    Error = "Failed to create stripe checkout."
                };
            }
        }

        public async Task<HTTPResponse<string, string>> HandleStripeEvent(Event stripeEvent)
        {
            HTTPResponse<string, string> result = new HTTPResponse<string, string>()
            {
                Success = false,
                HttpCode = 500,
                Error = "Failed to process stripe event"
            };

            _logger.LogInformation("Stripe Event Object:");
            _logger.LogInformation(JsonConvert.SerializeObject(stripeEvent));
            switch(stripeEvent.Type)
            {
                case EventTypes.InvoicePaymentSucceeded:
                    result = await HandleInvoicePaymentSucceedEvent(stripeEvent);
                    break;
                case EventTypes.CheckoutSessionCompleted:
                    result = await HandleCheckoutSessionCompletedEvent(stripeEvent);
                    break;
                // case EventTypes.customer:

                // case EventTypes.CustomerSubscriptionDeleted:
                //     result = await HandleSubscriptionCancelledEvent(stripeEvent);
                //     break;
                // default:
                //     _logger.LogInformation($"Stripe event {stripeEvent.Type} not supported by this method");
                //     result = new HTTPResponse<string, string>()
                //     {
                //         Success = false,
                //         HttpCode = 202
                //     };
                //     break;

            }

            return result;
        }

        private async Task<HTTPResponse<string, string>> HandleInvoicePaymentSucceedEvent(Event stripeEvent)
        {
            if (stripeEvent.Type != EventTypes.InvoicePaymentSucceeded)
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Incorrect event type",
                    HttpCode = 500
                };
            }

            if (stripeEvent.Data.Object is Invoice invoice)
            {
                // get tenant, check if it exists and is active
                // get meta data:
                invoice.Metadata.TryGetValue(TENANT_ID_META_DATA_KEY, out string tenantId);
                invoice.Metadata.TryGetValue(SUBSCRIPTION_TIER_ID_META_DATA_KEY, out string subscriptionTierId);

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
            
                // if (invoice.BillingReason == "subscription_created")
                // {
                //     return await _tenantOnboardingLogic.ActivateTenantWithSubscription(tenantId, subscriptionTierId, session); 
                // }
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Not implemented",
                    HttpCode = 500
                };

            }
            else
            {
                return new HTTPResponse<string, string>() {
                    Success = false,
                    HttpCode = 400,
                    Error = "Event data object isn't a subscription."
                };
            }
            return new HTTPResponse<string, string>() {
                Success = false,
                HttpCode = 400,
                Error = "Event data object isn't a subscription."
            };
        }

        private async Task<HTTPResponse<string, string>> HandleCheckoutSessionCompletedEvent(Event stripeEvent)
        {
            if (stripeEvent.Data.Object is Subscription subscription)
            {
                // get meta data:
                subscription.Metadata.TryGetValue(TENANT_ID_META_DATA_KEY, out string tenantId);
                subscription.Metadata.TryGetValue(SUBSCRIPTION_TIER_ID_META_DATA_KEY, out string subscriptionTierId);

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
            
                return await _tenantOnboardingLogic.ActivateTenantWithSubscription(tenantId, subscriptionTierId, subscription);            
            }
            else
            {
                return new HTTPResponse<string, string>()
                {
                    Success = false,
                    Error = "Stripe event object was not a session",
                    HttpCode = 500
                };
            }
        }

        private async Task<HTTPResponse<string, string>> HandleSubscriptionCancelledEvent(Event stripeEvent)
        {
            if (stripeEvent.Type != EventTypes.CustomerSubscriptionDeleted)
            {
                return new HTTPResponse<string, string>() {
                    Success = false,
                    HttpCode = 400,
                    Error = "Event type is wrong."
                };
            }

            if (stripeEvent.Data.Object is Subscription subscription)
            {
                // get tenant Id and then revoke access.
                subscription.Metadata.TryGetValue(TENANT_ID_META_DATA_KEY, out string tenantId);
                if (string.IsNullOrEmpty(tenantId))
                    {
                        return new HTTPResponse<string, string>() {
                        Success = false,
                        HttpCode = 400,
                        Error = "Event doesn't have correct meta data."
                    };
                } 

                var result = await _tenantOnboardingLogic.ScheduleTenantSubscriptionCancellation(tenantId);
                return result;
            }
            else
            {
                return new HTTPResponse<string, string>() {
                    Success = false,
                    HttpCode = 400,
                    Error = "Event data object isn't a subscription."
                };
            }



        }
    }
}