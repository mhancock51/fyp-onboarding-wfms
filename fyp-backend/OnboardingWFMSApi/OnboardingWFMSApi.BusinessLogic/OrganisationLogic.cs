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

        public OrganisationLogic(IOrganisationRepository organisationRepository, IOrganisationAdminLinkRepository organisationAdminLinkRepository)
        {
            _organisationRepository = organisationRepository;
            _organisationAdminLinkRepository = organisationAdminLinkRepository;
        }

        public async Task<HTTPResponse<string, string>> AssignAdminToOrganisation(string organisationId, string accountId)
        {
            try
            {
                await _organisationAdminLinkRepository.AddAsync(new OrganisationAdminLinkTable() { AccountId = accountId, OrganisationId = accountId });
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
