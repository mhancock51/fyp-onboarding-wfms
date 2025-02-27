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
    public interface IOrganisationLogic
    {
        public Task<HTTPResponse<string, string>> CreateOrganisation(string name);
        public Task<HTTPResponse<string, string>> AssignAdminToOrganisation(string organisationId, string accountId);
    }
    public class OrganisationLogic : IOrganisationLogic
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationAdminLinkRepository _organisationAdminLinkRepository;
        private readonly IAccountRepository _accountRepository;

        public OrganisationLogic(IOrganisationRepository organisationRepository, IOrganisationAdminLinkRepository organisationAdminLinkRepository)
        {
            _organisationRepository = organisationRepository;
            _organisationAdminLinkRepository = organisationAdminLinkRepository;
        }

        public async Task<HTTPResponse<string, string>> AssignAdminToOrganisation(string organisationId, string accountId)
        {            

            // check organisation exists
            var organisation = await _organisationRepository.GetById(organisationId);
            if (organisation == null)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Organisation doesn't exist", HttpCode = 400 };
            }
            
            // check account exists
            var account = await _accountRepository.GetById(accountId);
            if (account == null)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account doesn't exist", HttpCode = 400 };
            }

            // check if account is associated with organisation
            if (account.OrganisationId != organisationId)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account isn't associated with organisation", HttpCode = 400 };
            }      
            
            // check account isn't onboarder
            if (account.IsOnboarder)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account is an onboarder", HttpCode = 400 };
            }

            try
            {                
                await _organisationAdminLinkRepository.AddAsync(new OrganisationAdminLinkTable() { AccountId = accountId, OrganisationId = organisationId });
                // set account to admin
                account.IsAdmin = true;
                await _accountRepository.UpdateAsync(account);

                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Assigned admin" };
            }
            catch (Exception ex) 
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Failed to register admin" };           
            }

        }

        public async Task<HTTPResponse<string, string>> CreateOrganisation(string name)
        {
            await _organisationRepository.AddAsync(new OrganisationTable(){Name = name });
            return new HTTPResponse<string, string>() { Success = true, Data = "Created organisation", HttpCode = 200 };
        }
    }
}
