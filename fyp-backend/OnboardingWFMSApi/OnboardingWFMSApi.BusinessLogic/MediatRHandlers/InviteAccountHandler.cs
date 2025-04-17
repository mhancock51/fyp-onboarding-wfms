using MediatR;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class InviteAccountRequest : IRequest<HTTPResponse<string, string>>
    {
        public string displayName;
        public string emailAddress;
        public bool isSupervisor;
        public string departmentId;

        public InviteAccountRequest(string displayName, string emailAddress, string departmentId)
        {
            this.displayName = displayName;
            this.emailAddress = emailAddress;
            this.departmentId = departmentId;            
        }
    }

    public class InviteAccountHandler : IRequestHandler<InviteAccountRequest, HTTPResponse<string, string>>
    {
        private readonly IAccountLogic _accountLogic;
        private readonly ILogger<InviteAccountHandler> _logger;

        public InviteAccountHandler(IAccountLogic accountLogic, ILogger<InviteAccountHandler> logger)
        {
            _accountLogic = accountLogic;
            _logger = logger;
        }

        public async Task<HTTPResponse<string, string>> Handle(InviteAccountRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Handling request to invite user ({request.emailAddress}, {request.displayName})");
            return await _accountLogic.InviteUser(request.displayName, request.emailAddress,  request.departmentId);
        }
    }
}
