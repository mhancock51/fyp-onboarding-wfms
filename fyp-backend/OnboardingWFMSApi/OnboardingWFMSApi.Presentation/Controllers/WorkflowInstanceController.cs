using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/workflow/instance")]
    public class WorkflowInstanceController : ControllerBase
    {
        private readonly IWorkflowInstanceLogic _workflowInstanceLogic;

        public WorkflowInstanceController(IWorkflowInstanceLogic workflowInstanceLogic)
        {
            _workflowInstanceLogic = workflowInstanceLogic;
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> CreateIntance(CreateWorkflowInstancePayload payload)
        {
            var response = await _workflowInstanceLogic.CreateWorkflowInstance(payload);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetAccountsWorkflowInstances()
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _workflowInstanceLogic.GetAccountsWorkflowInstances(accountId);                    
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
