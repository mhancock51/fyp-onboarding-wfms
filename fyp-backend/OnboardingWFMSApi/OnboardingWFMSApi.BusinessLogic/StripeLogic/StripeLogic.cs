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


        #region Checkout event management
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

            var tenantHasSubscription = await _tenantOnboardingLogic.DoesTenantHaveSubscription(tenantId);                        
            if (tenantHasSubscription)
            {
                return new HTTPResponse<Session, string>()
                {
                    Success = false,
                    Error = "Tenant already has a subscription",
                    HttpCode = 400
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
                            StripeConstants.TENANT_ID_META_DATA_KEY, tenantId
                        },
                        {
                            StripeConstants.SUBSCRIPTION_TIER_ID_META_DATA_KEY, subscriptionTier.Id
                        }
                    },
                    Mode = "subscription", 
                    SuccessUrl = _checkoutSuccessUrl,
                    CancelUrl = _checkoutCancelUrl,
                };

                var client = new StripeClient(_stripeSecretKey);
                var service = new SessionService(client);
                Session session = await service.CreateAsync(options);

                _logger.LogInformation($"Successfully created checkout for tenant: {tenant.Id}, owned by: {tenant.OwnerAccountId}");

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
        #endregion

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
                    if (stripeEvent.Data.Object is Invoice invoice)
                    {
                        return await _tenantOnboardingLogic.ExtendTenantSubscription(invoice);                    
                    }
                    break;
                case EventTypes.CheckoutSessionCompleted:
                    if (stripeEvent.Data.Object is Subscription subscription)
                    {
                        return await _tenantOnboardingLogic.ActivateTenantWithSubscription(subscription);
                    }
                    break;
                case EventTypes.InvoicePaymentFailed:
                    if (stripeEvent.Data.Object is Invoice invc)
                    {
                        if (invc.Lines.Count() == 0) throw new Exception("Invoice has no line items");                        
                        var subscriptionLine = invc.Lines.First();

                        return await _tenantOnboardingLogic.ChangeTenantSubscriptionStatusToOverdue(subscriptionLine.SubscriptionId, invc.CustomerId);
                    }
                    break;
                case EventTypes.CustomerSubscriptionDeleted:
                    if (stripeEvent.Data.Object is Subscription subscription1)
                    {
                        return await _tenantOnboardingLogic.ChangeTenantSubscriptionStatusToOverdue(subscription1.Id, subscription1.CustomerId);
                    }
                    break;
                default:
                    _logger.LogInformation($"Stripe event {stripeEvent.Type} not supported by this method");
                    result = new HTTPResponse<string, string>()
                    {
                        Success = false,
                        HttpCode = 202
                    };
                    break;
            }

            return result;
        }
    }
}