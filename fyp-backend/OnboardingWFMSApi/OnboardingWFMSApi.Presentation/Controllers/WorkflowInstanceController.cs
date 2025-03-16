using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;
using OnboardingWFMSApi.DataModels.Payloads;

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
    }
}
