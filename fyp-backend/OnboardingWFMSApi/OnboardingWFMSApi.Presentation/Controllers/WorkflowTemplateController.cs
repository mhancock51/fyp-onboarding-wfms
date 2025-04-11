using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/workflow/template")]
    public class WorkflowTemplateController : ControllerBase
    {
        private readonly IWorkflowTemplateLogic _workflowTemplateLogic;

        public WorkflowTemplateController(IWorkflowTemplateLogic workflowTemplateLogic)
        {
            _workflowTemplateLogic = workflowTemplateLogic;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkflowTemplate([FromBody] CreateWorkflowTemplatePayload payload)
        {
            // TODO make endpoints supervisor only 
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _workflowTemplateLogic.CreateWorkflowTemplate(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }            
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateWorkflowTemplate([FromBody] CreateWorkflowTemplatePayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _workflowTemplateLogic.UpdateWorkflowTemplate(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpPost("archive")]
        public async Task<IActionResult> ArchiveWorkflowTemplate(string workflowTemplateId)
        {
            // TODO make this a supervisor only endpoint
            var response = await _workflowTemplateLogic.ArchiveWorkflowTemplate(workflowTemplateId);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetWorkflowTemplate(string workflowTemplateId)
        {
            var response = await _workflowTemplateLogic.GetWorkflowTemplate(workflowTemplateId);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllWorkflowTemplates()
        {
            var response = await _workflowTemplateLogic.GetAllWorkflowTemplates();
            return StatusCode(response.HttpCode, response);
        }
    }
}
