using Microsoft.AspNetCore.Mvc;
using OnboardingWFMSApi.BusinessLogic;

namespace OnboardingWFMSApi.Presentation.Controllers
{
    [ApiController]
    [Route("api/audit")]
    public class AuditController : ControllerBase
    {
        private readonly IWorkflowInstanceAuditLogic _workflowInstanceAuditLogic;

        public AuditController(IWorkflowInstanceAuditLogic workflowInstanceAuditLogic)
        {
            _workflowInstanceAuditLogic = workflowInstanceAuditLogic;
        }

        [HttpGet("workflow-instance-logs")]
        public async Task<IActionResult> GetWorkflowInstanceLogs(string workflowInstanceId)
        {
            var response = await _workflowInstanceAuditLogic.GetAuditLogsByWorkflowInstanceId(workflowInstanceId);
            return StatusCode(response.HttpCode, response);
        }
    }
}
