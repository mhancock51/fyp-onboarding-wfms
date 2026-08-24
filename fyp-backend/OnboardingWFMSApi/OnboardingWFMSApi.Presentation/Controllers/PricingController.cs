using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnboardingWFMSApi.BusinessLogic.StripeLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    /// <summary>
    /// Public pricing endpoint consumed by the FlowPath landing page.
    /// No authentication required. Rate-limited to prevent abuse.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PricingController : ControllerBase
    {
        private readonly IStripeLogic _stripeLogic;
        private readonly ILogger<PricingController> _logger;

        public PricingController(IStripeLogic stripeLogic, ILogger<PricingController> logger)
        {
            _stripeLogic = stripeLogic;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/pricing/tiers
        /// Returns active subscription tiers with live Stripe price data.
        /// Rate limited to 30 requests per minute per IP.
        /// </summary>
        [HttpGet("tiers")]
        [EnableRateLimiting("pricing-policy")]
        public async Task<IActionResult> GetTiers()
        {
            var response = await _stripeLogic.GetPublicPricingTiers();
            return StatusCode(response.HttpCode, response);
        }
    }
}
