using MediatR;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.DTOs;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.TaskInstanceLogic;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class CreateTaskInstanceRequest : IRequest<ServerResponse<string, string>>
    {
        public CreateTaskInstancePayload payload { get; set; }
        public CreateTaskInstanceRequest(CreateTaskInstancePayload payload)
        {
            this.payload = payload;
        }
    }

    public class CreateTaskInstanceHandler : IRequestHandler<CreateTaskInstanceRequest, ServerResponse<string, string>>
    {
        private readonly ITaskInstanceLogic _taskInstanceLogic;
        private readonly ILogger<CreateTaskInstanceHandler> _logger;

        public CreateTaskInstanceHandler(ITaskInstanceLogic taskInstanceLogic, ILogger<CreateTaskInstanceHandler> logger)
        {
            _taskInstanceLogic = taskInstanceLogic;
            _logger = logger;
        }

        public async Task<ServerResponse<string, string>> Handle(CreateTaskInstanceRequest request, CancellationToken cancellationToken)
        {
            var response = await _taskInstanceLogic.CreateInstance(request.payload);
            _logger.LogInformation($"Handled request to create task instance through mediator, response status: {response.Success}");
            return response;
        }
    }
}
