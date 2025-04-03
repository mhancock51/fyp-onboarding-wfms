using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Payloads;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/task/templates")]
    public class TaskTemplateController : ControllerBase
    {
        private readonly ITaskTemplateLogic _taskTemplateLogic;

        public TaskTemplateController(ITaskTemplateLogic taskTemplateLogic)
        {
            _taskTemplateLogic = taskTemplateLogic;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTemplates(string? status)
        {
            var response = await _taskTemplateLogic.GetAllTaskTemplates(status);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskTemplate([FromBody] CreateTaskTemplatePayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _taskTemplateLogic.CreateTaskTemplate(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
            
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetTaskTemplate(string id)
        {
            var response = await _taskTemplateLogic.GetTaskTemplateById(id);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet("task-types")]
        public async Task<IActionResult> GetAllTaskTypes()
        {
            var response = await _taskTemplateLogic.GetAllTaskTypes();
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpPost("archive")]
        public async Task<IActionResult> ArchiveTemplate(string taskTemplateId)
        {
            // TODO make this endpoint only accessible to supervisors
            var response = await _taskTemplateLogic.ArchiveTaskTemplate(taskTemplateId);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTemplate([FromBody] UpdateTaskTemplatePayload payload)
        {
            // TODO make this endpoint only accessible to supervisors
            var response = await _taskTemplateLogic.UpdateTaskTemplate(payload);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet("has-active-instances")]
        public async Task<IActionResult> GetHasActiveInstances(string taskTemplateId)
        {
            // TODO make this endpoint only accessible to supervisors
            var response = await _taskTemplateLogic.DoesTaskTemplateHaveActiveInstances(taskTemplateId);
            return StatusCode(response.HttpCode, response);
        }
    }
}
