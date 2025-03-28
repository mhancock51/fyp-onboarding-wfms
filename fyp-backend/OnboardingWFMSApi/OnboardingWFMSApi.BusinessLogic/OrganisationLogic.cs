using Microsoft.Extensions.Logging;
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
        public Task<HTTPResponse<OrganisationTable, string>> GetOrganisation();
        public Task<HTTPResponse<string, string>> RenameOrganisation(string newName);
    }
    public class OrganisationLogic : IOrganisationLogic
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationAdminLinkRepository _organisationAdminLinkRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<OrganisationLogic> _logger;

        public OrganisationLogic(IOrganisationRepository organisationRepository, IOrganisationAdminLinkRepository organisationAdminLinkRepository, IAccountRepository accountRepository, ILogger<OrganisationLogic> logger)
        {
            _organisationRepository = organisationRepository;
            _organisationAdminLinkRepository = organisationAdminLinkRepository;
            _accountRepository = accountRepository;
            _logger = logger;
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
           

            try
            {                
                await _organisationAdminLinkRepository.AddAsync(new OrganisationAdminLinkTable() { AccountId = accountId, OrganisationId = organisationId });
                // set account to admin
                account.IsSupervisor = true;
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
            try
            {
                await _organisationRepository.AddAsync(new OrganisationTable(){Name = name });
                return new HTTPResponse<string, string>() { Success = true, Data = "Created organisation", HttpCode = 200 };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Couldn't create organisation", HttpCode = 500 };
            }
        }

        public async Task<HTTPResponse<OrganisationTable, string>> GetOrganisation()
        {
            var orgs = await _organisationRepository.GetAll();
            if (orgs.Count() == 0)
            {
                return new HTTPResponse<OrganisationTable, string>() { Success = false, HttpCode = 400, Error = "No organisation exists" };
            }
            else
            {
                var org = orgs.First();
                return new HTTPResponse<OrganisationTable, string>() { Success = true, HttpCode = 200, Data = org };
            }
        }

        public async Task<HTTPResponse<string, string>> RenameOrganisation(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please provide a new name" };
            }

            var organisation = await _organisationRepository.GetById("organisation");
            organisation.Name = newName;
            try
            {
                await _organisationRepository.UpdateAsync(organisation);
                _logger.LogInformation($"Renamed organisation to {newName}");
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = $"Successfully renamed organisation to {organisation.Name}" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to rename organisation" };
            }

        }
    }
}
