using MediatR;
using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.MediatRHandlers
{
    public class RetrieveAccountsWorkflowInstancesRequest : IRequest<List<WorkflowInstanceDTO>>
    {
        public string AccountId { get; set; }

        public RetrieveAccountsWorkflowInstancesRequest(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class RetrieveAccountsWorkflowInstancesHandler : IRequestHandler<RetrieveAccountsWorkflowInstancesRequest, List<WorkflowInstanceDTO>>
    {
        private readonly IWorkflowInstanceLogic _workflowInstanceLogic;

        public RetrieveAccountsWorkflowInstancesHandler(IWorkflowInstanceLogic workflowInstanceLogic)
        {
            _workflowInstanceLogic = workflowInstanceLogic;
        }

        public async Task<List<WorkflowInstanceDTO>> Handle(RetrieveAccountsWorkflowInstancesRequest request, CancellationToken cancellationToken)
        {
            var response = await _workflowInstanceLogic.GetAccountsWorkflowInstances(request.AccountId);
            if (!response.Success || !response.HasData) return new List<WorkflowInstanceDTO>();

            return response.Data;
        }
    }
}
