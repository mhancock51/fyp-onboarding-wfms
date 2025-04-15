using MediatR;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class RetrieveAccountDirectoryRequest : IRequest<AccountDirectoryDTO>
    {
        public string AccountId { get; set; }
        public RetrieveAccountDirectoryRequest(string accountId)
        {
            this.AccountId = accountId;
        }
    }

    public class RetrieveAccountDirectoryHandler : IRequestHandler<RetrieveAccountDirectoryRequest, AccountDirectoryDTO>
    {
        private readonly IAccountLogic _accountLogic;

        public RetrieveAccountDirectoryHandler(IAccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        public async Task<AccountDirectoryDTO> Handle(RetrieveAccountDirectoryRequest request, CancellationToken cancellationToken)
        {
            return await _accountLogic.GetDirectoryByAccountId(request.AccountId);
        }
    }
}
