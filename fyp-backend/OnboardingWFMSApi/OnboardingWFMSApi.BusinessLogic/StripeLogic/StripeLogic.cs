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
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables;
using Stripe;
using Stripe.Checkout;

namespace OnboardingWFMSApi.BusinessLogic.StripeLogic
{
    public interface IStripeLogic
    {
        public Task<HTTPResponse<Session, string>> CreateCheckoutSession(string tenantId, string tierId);
        public Task<HTTPResponse<string, string>> HandleStripeEvent(Event stripeEvent);
        public Task<HTTPResponse<List<PricingTierDTO>, string>> GetPublicPricingTiers();
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

        private readonly ITenantLogic _tenantOnboardingLogic;

        public StripeLogic(
            ILogger<StripeLogic> logger,
            ITenantRepository tenantRepository,
            ISubscriptionTierRepository subscriptionTierRepository,
            IConfiguration configuration,
            ITenantLogic tenantOnboardingLogic
        )
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

            _logger.LogInformation($"Handling stripe event, event type: {stripeEvent.Type}");

            switch(stripeEvent.Type)
            {
                case EventTypes.InvoicePaymentSucceeded:
                    if (stripeEvent.Data.Object is Invoice invoice)
                    {                        
                        return await _tenantOnboardingLogic.ExtendTenantSubscription(invoice);                                            
                    }
                    break;
                case EventTypes.CheckoutSessionCompleted:                    
                    if (stripeEvent.Data.Object is Session session)
                    {
                        return await _tenantOnboardingLogic.ActivateTenantWithSubscription(session);
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
                        string subscriptionId = subscription1.Items.First().Subscription;
                        return await _tenantOnboardingLogic.DeleteTenantSubscription(subscriptionId, subscription1.CustomerId);
                    }
                    break;
                case EventTypes.CustomerSubscriptionUpdated:
                    if (stripeEvent.Data.Object is Subscription subscription2)
                    {                        
                        if (subscription2.Status == "unpaid")
                        {                                                        
                            string subscriptionId = subscription2.Items.First().Subscription;
                            return await _tenantOnboardingLogic.DeleteTenantSubscription(subscriptionId, subscription2.CustomerId);
                        }
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

        #region Public pricing (for landing page)
        /// <summary>
        /// Returns active subscription tiers with live Stripe price data
        /// and feature lists for display on the public landing page.
        /// </summary>
        public async Task<HTTPResponse<List<PricingTierDTO>, string>> GetPublicPricingTiers()
        {
            try
            {
                // 1. Get all active tiers from the database
                var allTiers = await _subscriptionTierRepository.GetAll();
                var activeTiers = allTiers.Where(t => t.IsActive && t.Id != "admin-tier").ToList();

                // 2. Build a feature map keyed by tier Id
                var featureMap = GetTierFeatureMap();

                // 3. Create a Stripe client for fetching live price data
                var stripeClient = new StripeClient(_stripeSecretKey);
                var priceService = new PriceService(stripeClient);

                // 4. For each tier, try to fetch the Stripe Price
                var result = new List<PricingTierDTO>();
                foreach (var tier in activeTiers)
                {
                    var dto = new PricingTierDTO
                    {
                        Id = tier.Id,
                        Name = tier.Id,
                        DisplayName = tier.DisplayName,
                        Description = GetTierDescription(tier.Id),
                        Highlighted = tier.Id == "tier-2-subscription",
                        CtaText = tier.Id == "tier-3-subscription" ? "Contact Sales" : "Get started today",
                        Features = featureMap.TryGetValue(tier.Id, out var features)
                            ? features
                            : new List<string>()
                    };

                    // Try to fetch live price from Stripe
                    if (!string.IsNullOrWhiteSpace(tier.PriceId))
                    {
                        try
                        {
                            var stripePrice = await priceService.GetAsync(tier.PriceId);
                            dto.UnitAmount = stripePrice.UnitAmount;
                            dto.Currency = stripePrice.Currency?.ToUpperInvariant() ?? "USD";
                            dto.Interval = stripePrice.Recurring?.Interval ?? "month";

                            dto.PriceDisplay = FormatPriceDisplay(
                                stripePrice.UnitAmount,
                                stripePrice.Currency,
                                stripePrice.Recurring?.Interval,
                                stripePrice.Recurring?.IntervalCount ?? 1);
                        }
                        catch (StripeException ex)
                        {
                            _logger.LogWarning(
                                "Failed to fetch Stripe price {PriceId} for tier {TierId}: {Message}",
                                tier.PriceId, tier.Id, ex.Message);
                            // Fall back to a placeholder so the page still renders
                            dto.PriceDisplay = "Contact us";
                        }
                    }
                    else
                    {
                        // Enterprise / custom-priced tier
                        dto.PriceDisplay = "Custom";
                        dto.Interval = string.Empty;
                    }

                    result.Add(dto);
                }

                return new HTTPResponse<List<PricingTierDTO>, string>
                {
                    Success = true,
                    HttpCode = 200,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build public pricing tiers");
                return new HTTPResponse<List<PricingTierDTO>, string>
                {
                    Success = false,
                    HttpCode = 500,
                    Error = "Unable to retrieve pricing information."
                };
            }
        }

        private static string FormatPriceDisplay(long? unitAmount, string? currency, string? interval, long intervalCount)
        {
            if (unitAmount == null) return "Contact us";

            var symbol = (currency?.ToUpperInvariant()) switch
            {
                "USD" => "$",
                "EUR" => "€",
                "GBP" => "£",
                "CAD" => "CA$",
                "AUD" => "A$",
                _ => (currency ?? "") + " "
            };

            var amount = (decimal)unitAmount.Value / 100m;
            var amountStr = amount % 1 == 0 ? $"{amount:0}" : $"{amount:0.00}";

            var intervalStr = interval?.ToLowerInvariant() switch
            {
                "month" when intervalCount == 1 => "/month",
                "month" => $"/{intervalCount} months",
                "year" when intervalCount == 1 => "/year",
                "year" => $"/{intervalCount} years",
                _ => ""
            };

            return $"{symbol}{amountStr}{intervalStr}";
        }

        private static string GetTierDescription(string tierId) => tierId switch
        {
            "tier-1-subscription" => "Perfect for small teams getting started with structured onboarding.",
            "tier-2-subscription" => "For growing companies that need advanced workflows and integrations.",
            "tier-3-subscription" => "For large organizations with complex, multi-department onboarding needs.",
            _ => ""
        };

        private static Dictionary<string, List<string>> GetTierFeatureMap() => new()
        {
            ["tier-1-subscription"] = new List<string>
            {
                "Up to 5 workflow templates",
                "10 task templates",
                "5 active onboardings",
                "Basic document storage (100 MB)",
                "Email support"
            },
            ["tier-2-subscription"] = new List<string>
            {
                "Unlimited workflow templates",
                "Unlimited task templates",
                "50 active onboardings",
                "1 GB document storage",
                "Custom branding",
                "API access & webhooks",
                "Priority support"
            },
            ["tier-3-subscription"] = new List<string>
            {
                "Everything in Professional",
                "Unlimited active onboardings",
                "Unlimited document storage",
                "SSO / SAML / OAuth",
                "Dedicated tenant isolation",
                "Custom integrations",
                "Dedicated account manager"
            },
            ["admin-tier"] = new List<string>
            {
                "All features",
                "Unlimited everything",
                "Full administrative access"
            }
        };

        #endregion
    }
}