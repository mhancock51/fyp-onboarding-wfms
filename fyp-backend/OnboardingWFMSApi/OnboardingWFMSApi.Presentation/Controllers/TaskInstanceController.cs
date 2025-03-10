using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System.Security.Claims;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/task/instances")]
    public class TaskInstanceController : ControllerBase
    {
        private readonly ITaskInstanceLogic _taskInstanceLogic;

        public TaskInstanceController(ITaskInstanceLogic taskInstanceLogic)
        {
            _taskInstanceLogic = taskInstanceLogic;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateInstance([FromBody] CreateTaskInstancePayload payload)
        {
            var response = await _taskInstanceLogic.CreateInstance(payload);
            return StatusCode(response.HttpCode, response);
        }

        [Authorize]
        [HttpGet("get-assigned")]
        public async Task<IActionResult> GetAssigned()
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _taskInstanceLogic.GetUsersAssignedTask(accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpPost("update-task-state/checklist")]
        public async Task<IActionResult> UpdateChecklistInstanceState([FromBody] UpdateInstanceStateChecklistPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _taskInstanceLogic.UpdateChecklistInstanceState(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }            
        }

        [Authorize]
        [HttpPost("update-task-state/read-doc")]
        public async Task<IActionResult> UpdateReadDocInstanceState([FromBody] UpdateInstanceStateReadDocPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _taskInstanceLogic.UpdateReadDocumentInstanceState(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [Authorize]
        [HttpPost("update-task-state/upload-doc")]
        public async Task<IActionResult> UpdateUploadDocInstanceState([FromBody] UpdateInstanceStateFileUploadPayload payload)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _taskInstanceLogic.UpdateFileUploadInstanceState(payload, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteInstance(string taskInstanceId)
        {
            string accountId = UserIdentityUtils.GetAccountIdFromClaimIdentity(User.Identity as ClaimsIdentity);
            if (accountId == "")
            {
                var response = new HTTPResponse<string, string>() { Success = false, HttpCode = 401, Message = "Invalid credentials" };
                return StatusCode(response.HttpCode, response);
            }
            else
            {
                var response = await _taskInstanceLogic.CompleteTaskInstance(taskInstanceId, accountId);
                return StatusCode(response.HttpCode, response);
            }
        }
    }
}
