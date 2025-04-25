using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic;
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
        
        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateIntance(CreateWorkflowInstancePayload payload)
        {
            var response = await _workflowInstanceLogic.CreateWorkflowInstance(payload);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet("all")]
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetWorkflowInstance(string workflowInstanceId)
        {
            var response = await _workflowInstanceLogic.GetWorkflowInstanceDTO(workflowInstanceId);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize(Policy = "SupervisorRoleClaim")]
        [HttpGet("onboarding/open")]
        public async Task<IActionResult> GetAllOpenWorkflowInstances(DateTime? from, DateTime? to) 
        {
            var response = await _workflowInstanceLogic.GetAlllOnboardingWorkflowInstances(from, to);
            return StatusCode(response.HttpCode, response);
        }
    }
}
