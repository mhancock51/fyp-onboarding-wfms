using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels.Payloads;
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
        public async Task<IActionResult> GetAssigned(string accountId)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;            
            var response = await _taskInstanceLogic.GetUsersAssignedTask(accountId);
            return StatusCode(response.HttpCode, response);
        }
    }
}
