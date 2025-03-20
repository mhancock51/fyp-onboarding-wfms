using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class TaskCompletedRequest : IRequest<HTTPResponse<string, string>>
    {
        public TaskInstanceDTO taskInstance;
        public TaskCompletedRequest(TaskInstanceDTO taskInstance) 
        {
            this.taskInstance = taskInstance;
        }
    }

    public class TaskCompletionHandler : IRequestHandler<TaskCompletedRequest, HTTPResponse<string, string>>
    {
        private readonly IWorkflowInstanceLogic _workflowInstanceLogic;
        private readonly ILogger<TaskCompletionHandler> _logger;

        public TaskCompletionHandler(IWorkflowInstanceLogic workflowInstanceLogic, ILogger<TaskCompletionHandler> logger)
        {
            _workflowInstanceLogic = workflowInstanceLogic;
            _logger = logger;
        }

        public async Task<HTTPResponse<string, string>> Handle(TaskCompletedRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Handling task completed event for task instance: {request.taskInstance.Id}");
            if (!string.IsNullOrEmpty(request.taskInstance.WorkflowInstanceId))
            {
                // call workflow instance to handle completion
                var result = await _workflowInstanceLogic.HandleTaskInstanceCompletion(request.taskInstance);
                if (!result.Success)
                {
                    return result;
                }
            }

            return new HTTPResponse<string, string>() { Success = true, Data = "Success", HttpCode = 200 };
        }
    }
}
