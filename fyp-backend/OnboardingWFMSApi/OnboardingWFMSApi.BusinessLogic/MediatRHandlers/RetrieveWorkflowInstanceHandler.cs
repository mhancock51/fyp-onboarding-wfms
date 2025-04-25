using MediatR;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class RetrieveWorkflowInstanceRequest : IRequest<ServerResponse<WorkflowInstanceDTO, string>>
    {
        public string WorkflowInstanceId { get; set; }
        public RetrieveWorkflowInstanceRequest(string workflowInstanceId)
        {
            WorkflowInstanceId = workflowInstanceId;            
        }
    }

    public class RetrieveWorkflowInstanceHandler : IRequestHandler<RetrieveWorkflowInstanceRequest, ServerResponse<WorkflowInstanceDTO, string>>
    {
        private readonly IWorkflowInstanceLogic _workflowInstanceLogic;
        private readonly ILogger<RetrieveWorkflowInstanceHandler> _logger;

        public RetrieveWorkflowInstanceHandler(IWorkflowInstanceLogic workflowInstanceLogic, ILogger<RetrieveWorkflowInstanceHandler> logger)
        {
            _workflowInstanceLogic = workflowInstanceLogic;
            _logger = logger;
        }

        public async Task<ServerResponse<WorkflowInstanceDTO, string>> Handle(RetrieveWorkflowInstanceRequest request, CancellationToken cancellationToken)
        {
            var response = await _workflowInstanceLogic.GetWorkflowInstance(request.WorkflowInstanceId);
            _logger.LogInformation($"Retrieve response to get workflow instance, success: {response.Success}");
            return response;
        }
    }
}
