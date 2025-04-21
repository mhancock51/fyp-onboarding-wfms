using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IAccountUtility
    {
        public Task<string> ReplaceAccountIdPlaceholder(string placeholderId, WorkflowInstanceTable workflowInstance);
    }

    public class AccountUtility : IAccountUtility
    {
        public const string ONBOARDER_ACCOUNT_ID_PLACEHOLDER = "onboarder_account_id";
        public const string SUPERVISOR_ACCOUNT_ID_PLACEHOLDER = "supervisors_account_id";

        private readonly IOnboardingEmployeeDetailsRepository _onboardingEmployeeDetailsRepository;

        public AccountUtility(IOnboardingEmployeeDetailsRepository onboardingEmployeeDetailsRepository)
        {
            _onboardingEmployeeDetailsRepository = onboardingEmployeeDetailsRepository;
        }

        /// <summary>
        /// Takes an account Id, if accountId matches a placeholder, it returns the appropriate account Id counterpart.
        /// For example "onboarder_account_id" will return the account Id of the workflow instance's onboarder
        /// </summary>
        /// <param name="placeholderName"></param>
        /// <returns></returns>
        public async Task<string> ReplaceAccountIdPlaceholder(string placeholderId, WorkflowInstanceTable workflowInstance)
        {
            switch (placeholderId)
            {
                case ONBOARDER_ACCOUNT_ID_PLACEHOLDER:
                    var onboardingEmployeeDetails = await _onboardingEmployeeDetailsRepository.GetDetailsByWorkflowInstance(workflowInstance.Id);
                    if (string.IsNullOrEmpty(onboardingEmployeeDetails?.OnboarderAccountId)) throw new Exception("Onboarder account Id not set for workflow instance");
                    return onboardingEmployeeDetails.OnboarderAccountId;
                case SUPERVISOR_ACCOUNT_ID_PLACEHOLDER:
                    if (string.IsNullOrEmpty(workflowInstance.SupervisorAccountId)) throw new Exception("Supervisor account Id not set for workflow instance");
                    return workflowInstance.SupervisorAccountId;
                default:
                    return placeholderId;
            }
        }
    }
}
