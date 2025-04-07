using MediatR;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class CreateWorkflowInstanceAuditLogRequest : IRequest<ServerResponse<string, string>>
    {
        public CreateWorkflowInstanceAuditLogPayload payload { get; set; }
        public CreateWorkflowInstanceAuditLogRequest(CreateWorkflowInstanceAuditLogPayload payload)
        {
            this.payload = payload;
        }
    }

    public class CreateWorkflowInstanceAuditLogHandler : IRequestHandler<CreateWorkflowInstanceAuditLogRequest, ServerResponse<string, string>>
    {
        private readonly IWorkflowInstanceAuditLogic _workflowInstanceAuditLogic;

        public CreateWorkflowInstanceAuditLogHandler(IWorkflowInstanceAuditLogic workflowInstanceAuditLogic)
        {
            _workflowInstanceAuditLogic = workflowInstanceAuditLogic;
        }

        public async Task<ServerResponse<string, string>> Handle(CreateWorkflowInstanceAuditLogRequest request, CancellationToken cancellationToken)
        {
            return await _workflowInstanceAuditLogic.CreateLog(request.payload);
        }
    }
}
