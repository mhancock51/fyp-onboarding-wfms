using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic.StripeLogic;
using OnboardingWFMSApi.DataAccess;
using Stripe;
using Stripe.Checkout;

namespace OnboardingWFMSApi.Presentation.Controllers.TenantManagement
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IStripeLogic _stripeLogic;
        private readonly ICurrentTenantService _currentTenantService;
        private readonly string _webhookSecret;

        private readonly ILogger<StripeController> _logger;

        public StripeController(IStripeLogic stripeLogic, ICurrentTenantService currentTenantService, IConfiguration configuration, ILogger<StripeController> logger)
        {
            _stripeLogic = stripeLogic;
            _currentTenantService = currentTenantService;
            _webhookSecret = configuration["Stripe:WebhookSecret"] ?? "";
            _logger = logger;
        }


        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            if (string.IsNullOrEmpty(request.SubscriptionTierId))
            {
                return BadRequest(new { error = "Subscription tier is required." });
            }

            if (_currentTenantService.TenantId == null)
            {
                return BadRequest(new { error = "Tenant Id is required." });
            }

            var response = await _stripeLogic.CreateCheckoutSession(_currentTenantService.TenantId, request.SubscriptionTierId);
            return StatusCode(response.HttpCode, response);
        }

        public class CheckoutRequest
        {
            public string SubscriptionTierId { get; set; }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();

            _logger.LogInformation($"Stripe webhook hit");

            try
            {
                // 2. Extract the signature header sent by Stripe
                var stripeSignature = Request.Headers["Stripe-Signature"];

                // 3. Verify the event came genuinely from Stripe and wasn't tampered with
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _webhookSecret,
                    throwOnApiVersionMismatch: false
                );
                var result = await _stripeLogic.HandleStripeEvent(stripeEvent);
                return StatusCode(result.HttpCode, result.Data);
            }
            catch (StripeException e)
            {
                _logger.LogError($"Stripe exception raised: {e.Message}");
                // Invalid signature, bad payload, etc.
                return BadRequest(new { error = e.Message });
            }
        }
    }
}