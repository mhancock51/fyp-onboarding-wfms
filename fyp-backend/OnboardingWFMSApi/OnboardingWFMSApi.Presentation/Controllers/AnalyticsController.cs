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

        [HttpGet("workflow-instances/completed")]
        public async Task<IActionResult> GetOpenInstances(DateTime? from)
        {
            var response = await _analyticsLogic.GetCompletedWorkflowInstances(from);
            return StatusCode(response.HttpCode, response);
        }

        [HttpGet("workflow-instances/open")]
        public async Task<IActionResult> GetCompletedInstances()
        {
            var response = await _analyticsLogic.GetOpenWorkflowInstances();
            return StatusCode(response.HttpCode, response);
        }

        [HttpGet("workflow-instances/average")]
        public async Task<IActionResult> GetAvgTimeToOnboard()
        {
            var response = await _analyticsLogic.GetAverageTimeToOnboard();
            return StatusCode(response.HttpCode, response);
        }

    }
}
