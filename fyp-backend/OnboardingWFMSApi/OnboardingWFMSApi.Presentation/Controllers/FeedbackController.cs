using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.BusinessLogic.TaskInstanceLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class FeedbackController : ControllerBase
    {
        private readonly ITaskInstanceLogic _taskInstanceLogic;
        private readonly IFeedbackLogic _feedbackLogic;

        public FeedbackController(ITaskInstanceLogic taskInstanceLogic, IFeedbackLogic feedbackLogic)
        {
            _taskInstanceLogic = taskInstanceLogic;
            _feedbackLogic = feedbackLogic;
        }

        [HttpGet("worfklow-instance")]
        public async Task<IActionResult> GetFeedbackForWorkflowInstance(string workflowInstanceId)
        {
            var response = await _taskInstanceLogic.GetWorkflowInstanceCompletedFeedbackTasks(workflowInstanceId);
            return StatusCode(response.HttpCode, response);
        }

        [HttpGet("workflow-template")]
        public async Task<IActionResult> GetFeedbackForWorkflowTemplate(string workflowTemplateId)
        {
            var response = await _feedbackLogic.GetFeedbackTaskInstancesByWorkflowTemplate(workflowTemplateId);
            return StatusCode(response.HttpCode, response);
        }
             
    }
}
