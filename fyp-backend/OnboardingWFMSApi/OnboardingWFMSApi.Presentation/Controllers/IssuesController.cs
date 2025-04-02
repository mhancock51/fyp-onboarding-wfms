using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/issues")]
    public class IssuesController : ControllerBase
    {
        private readonly IReportedIssuesLogic _reportedIssuesLogic;

        public IssuesController(IReportedIssuesLogic reportedIssuesLogic)
        {
            _reportedIssuesLogic = reportedIssuesLogic;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateIssue([FromBody] CreateIssuePayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _reportedIssuesLogic.CreateIssue(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }            
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllIssues()
        {
            var response = await _reportedIssuesLogic.GetAllIssues();
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdatedIssueStatus([FromBody] UpdateIssueStatusPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _reportedIssuesLogic.UpdateIssueStatus(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
