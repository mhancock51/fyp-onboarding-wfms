using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

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

        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser(string displayName, string emailAddress, string departmentId)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return BadRequest("Please enter a display name");
            }
            if (string.IsNullOrEmpty(emailAddress)) 
            {
                return BadRequest("Please enter an email address");
            }
            if (string.IsNullOrEmpty(departmentId))
            {
                return BadRequest("Please select a department");
            }

            var result = await _accountLogic.InviteUser(displayName, emailAddress, departmentId);
            return StatusCode(result.HttpCode, result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(string emailAddress, string password, string confirmationPassword)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(confirmationPassword))
            {
                return BadRequest();
            }

            var result = await _accountLogic.RegisterUser(emailAddress, password, confirmationPassword);
            return StatusCode(result.HttpCode, result);
        }

        [HttpGet("get-invited-account")]
        public async Task<IActionResult> GetInvitedAccount(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Email address must be provided" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _accountLogic.GetInvitedAccount(emailAddress);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _accountLogic.DeleteAccount(accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpGet("directory")]
        public async Task<IActionResult> GetDirectory()
        {
            var response = await _accountLogic.GetDirectoryOfAllRegisteredAccounts();
            return StatusCode(response.HttpCode, response);
        }

        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpPost("make-supervisor")]
        public async Task<IActionResult> MakeAccountSupervisor(string accountId)
        {
            var response = await _accountLogic.MakeSupervisor(accountId);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccountDetails([FromBody] UpdateAccountPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _accountLogic.UpdateAccount(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _accountLogic.UpdatePassword(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
