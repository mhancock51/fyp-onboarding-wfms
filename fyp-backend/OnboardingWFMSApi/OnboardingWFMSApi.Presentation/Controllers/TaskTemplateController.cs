using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels.Payloads;

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
        public async Task<IActionResult> GetAllTemplates()
        {
            var response = await _taskTemplateLogic.GetAllTaskTemplates();
            return StatusCode(response.HttpCode, response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTaskTemplate([FromBody] CreateTaskTemplatePayload payload)
        {
            var response = await _taskTemplateLogic.CreateTaskTemplate(payload);
            return StatusCode(response.HttpCode, response);
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetTaskTemplate(string id)
        {
            var response = await _taskTemplateLogic.GetTaskTemplateById(id);
            return StatusCode(response.HttpCode, response);
        }
    }
}
