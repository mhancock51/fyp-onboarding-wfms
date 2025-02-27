using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IAccountLogic
    {
        public Task<HTTPResponse<string, string>> InviteUser(string displayName, string emailAddress, bool isOnboarder, string departmentId, string organisationId);        
    }

    public class AccountLogic : IAccountLogic
    {
        public const string INVITED_STATUS = "invited";

        private readonly IAccountRepository _accountRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IOrganisationRepository _organisationRepository;

        public AccountLogic(IAccountRepository accountRepository, IDepartmentRepository departmentRepository, IOrganisationRepository organisationRepository)
        {
            _accountRepository = accountRepository;
            _departmentRepository = departmentRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<HTTPResponse<string, string>> InviteUser(string displayName, string emailAddress, bool isOnboarder, string departmentId, string organisationId)
        {           
            // check email address doesn't already exist
            var existingAccount = await _accountRepository.GetByEmailAddress(emailAddress);
            if (existingAccount != null)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account already exists", HttpCode = 400 };
            }

            // check organisation exists
            if (!await _organisationRepository.ExistsById(organisationId))
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Organisation doesn't exist", HttpCode = 400 };
            }

            // check department exists
            if (!await _departmentRepository.ExistsById(departmentId))
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Department doesn't exist", HttpCode = 400 };
            }            

            // insert user record and set status to invited
            AccountTable account = new AccountTable()
            {
                EmailAddress = emailAddress,
                IsOnboarder = isOnboarder,
                IsAdmin = false,
                OrganisationId = organisationId,
                DepartmentId = departmentId,
                DisplayName = displayName,
                HashedPassword = "",
                AccountStatus = INVITED_STATUS
            };
            try
            {
                await _accountRepository.AddAsync(account);
                // TODO send invite link to user
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = $"Successfully invited {displayName}" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Failed to invite user" };
            }

        }
    }
}
