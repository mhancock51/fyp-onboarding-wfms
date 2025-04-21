using MediatR;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.BusinessLogic.WorkflowInstanceLogic;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class AccountRegistrationRequest : IRequest<HTTPResponse<string, string>>
    {
        public string accountId;
        public string emailAddress;
        public AccountRegistrationRequest(string accountId, string emailAddress)
        {
            this.accountId = accountId;
            this.emailAddress = emailAddress;
        }
    }

    public class AccountRegistrationHandler : IRequestHandler<AccountRegistrationRequest, HTTPResponse<string, string>>
    {
        private readonly IWorkflowInstanceLogic _workflowInstanceLogic;

        public AccountRegistrationHandler(IWorkflowInstanceLogic workflowInstanceLogic)
        {
            _workflowInstanceLogic = workflowInstanceLogic;
        }

        public async Task<HTTPResponse<string, string>> Handle(AccountRegistrationRequest request, CancellationToken cancellationToken)
        {
            var result = await _workflowInstanceLogic.HandleOnboarderRegistration(request.accountId, request.emailAddress);
            if (!result.Success) return result;

            return new HTTPResponse<string, string>() { Success = true };
        }
    }
}
