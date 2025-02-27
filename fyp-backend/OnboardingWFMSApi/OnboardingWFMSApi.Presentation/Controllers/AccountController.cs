using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountLogic _accountLogic;

        public AccountController(IAccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        [HttpGet("invite")]
        public async Task<IActionResult> InviteUser(string displayName, string emailAddress, bool isOnboarder, string departmentId)
        {
            // TODO make organisation ID derived from user context
            var result = await _accountLogic.InviteUser(displayName, emailAddress, isOnboarder, departmentId, "organisation");
            return StatusCode(result.HttpCode, result);
        }
    }
}
