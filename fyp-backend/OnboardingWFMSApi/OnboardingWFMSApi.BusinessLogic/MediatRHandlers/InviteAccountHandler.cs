using MediatR;
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
        public bool isOnboarder;
        public string departmentId;

        public InviteAccountRequest(string displayName, string emailAddress, bool isOnboarder, string departmentId)
        {
            this.displayName = displayName;
            this.emailAddress = emailAddress;
            this.isOnboarder = isOnboarder;
            this.departmentId = departmentId;            
        }
    }

    public class InviteAccountHandler : IRequestHandler<InviteAccountRequest, HTTPResponse<string, string>>
    {
        private readonly IAccountLogic _accountLogic;

        public InviteAccountHandler(IAccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        public async Task<HTTPResponse<string, string>> Handle(InviteAccountRequest request, CancellationToken cancellationToken)
        {
            return await _accountLogic.InviteUser(request.displayName, request.emailAddress, request.isOnboarder, request.departmentId);
        }
    }
}
