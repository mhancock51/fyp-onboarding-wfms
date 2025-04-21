using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Authorize(Policy = "SupervisorRoleClaim")]
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

        [HttpGet("onboarding/timeline")]
        public async Task<IActionResult> GetOnboardedEmployeesTimeline()
        {
            var response = await _analyticsLogic.GetOnboardedEmployeesTimeline();
            return StatusCode(response.HttpCode, response);
        }

        [HttpGet("reported-issues")]
        public async Task<IActionResult> GetReportedIssuesAnalytics(DateTime? from)
        {
            var response = await _analyticsLogic.GetReportedIssuesAnalytics(from);
            return StatusCode(response.HttpCode, response);
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetTaskAnalytics(DateTime? from)
        {
            var response = await _analyticsLogic.GetTaskAnalytics(from);
            return StatusCode(response.HttpCode, response);
        }
    }
}
