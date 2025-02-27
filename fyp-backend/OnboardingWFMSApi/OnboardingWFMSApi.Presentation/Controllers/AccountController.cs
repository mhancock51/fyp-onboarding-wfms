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

        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser(string displayName, string emailAddress, bool isOnboarder, string departmentId)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(emailAddress)) 
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest();
            }

            // TODO make organisation ID derived from user context
            var result = await _accountLogic.InviteUser(displayName, emailAddress, isOnboarder, departmentId, "organisation");
            return StatusCode(result.HttpCode, result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(string emailAddress, string hashedPassword, string hashedConfirmationPassword)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(hashedPassword))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(hashedConfirmationPassword))
            {
                return BadRequest();
            }

            var result = await _accountLogic.RegisterUser(emailAddress, hashedPassword, hashedConfirmationPassword);
            return StatusCode(result.HttpCode, result);
        }
    }
}
