using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsLogic _analyticsLogic;

        public AnalyticsController(IAnalyticsLogic analyticsLogic)
        {
            _analyticsLogic = analyticsLogic;
        }

        [HttpGet("onboarding")]
        public async Task<IActionResult> GetOnboardingAnalytics(DateTime? from)
        {
            var response = await _analyticsLogic.GetOnboardingAnalytics(from);
            return StatusCode(response.HttpCode, response);
        }
    }
}
