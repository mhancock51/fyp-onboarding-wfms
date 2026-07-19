using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    }

    public class StripeLogic : IStripeLogic
    {
        private readonly ILogger<StripeLogic> _logger;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionTierRepository _subscriptionTierRepository;

        public StripeLogic(ILogger<StripeLogic> logger, ITenantRepository tenantRepository, ISubscriptionTierRepository subscriptionTierRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _subscriptionTierRepository = subscriptionTierRepository;
        }

        public async Task<HTTPResponse<Session, string>> CreateCheckoutSession(string tenantId, string tierId)
        {
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
                    Mode = "subscription", 
                    SuccessUrl = $"/",
                    CancelUrl = $"/",
                };

                var service = new SessionService();
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
    }
}