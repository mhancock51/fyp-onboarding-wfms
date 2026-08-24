using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels.Payloads;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetStartedController : ControllerBase
    {
        private readonly IGetStartedLogic _getStartedLogic;
        private readonly ILogger<GetStartedController> _logger;

        public GetStartedController(IGetStartedLogic getStartedLogic, ILogger<GetStartedController> logger)
        {
            _getStartedLogic = getStartedLogic;
            _logger = logger;
        }

        [HttpPost]
        [EnableRateLimiting("get-started-policy")]
        public async Task<IActionResult> GetStarted([FromBody] GetStartedPayload payload)
        {
            if (payload == null)
                return BadRequest(new { error = "Request body is required." });

            var response = await _getStartedLogic.GetStarted(payload);
            return StatusCode(response.HttpCode, response);
        }
    }
}
