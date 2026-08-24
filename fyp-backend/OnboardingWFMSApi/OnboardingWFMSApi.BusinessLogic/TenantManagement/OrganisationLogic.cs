using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
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
        public Task<HTTPResponse<OrganisationTable, string>> GetOrganisation();
        public Task<HTTPResponse<string, string>> RenameOrganisation(string newName);
        public Task<HTTPResponse<string, string>> UpdateOrganisationLogo(IFormFile logoFile);
    }
    public class OrganisationLogic : IOrganisationLogic
    {
        private readonly IOrganisationRepository _organisationRepository;        
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<OrganisationLogic> _logger;

        public OrganisationLogic(IOrganisationRepository organisationRepository, IAccountRepository accountRepository, ILogger<OrganisationLogic> logger)
        {
            _organisationRepository = organisationRepository;            
            _accountRepository = accountRepository;
            _logger = logger;
        }
        public async Task<HTTPResponse<string, string>> CreateOrganisation(string name)
        {
            try
            {
                await _organisationRepository.AddAsync(new OrganisationTable(){Name = name });
                return new HTTPResponse<string, string>() { Success = true, Data = "Created organisation", HttpCode = 200 };
            }
            catch (Exception)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Couldn't create organisation", HttpCode = 500 };
            }
        }

        public async Task<HTTPResponse<OrganisationTable, string>> GetOrganisation()
        {
            var org = await GetCurrentOrganisation();
            if (org == null)
            {
                return new HTTPResponse<OrganisationTable, string>() { Success = false, HttpCode = 400, Error = "No organisation exists" };
            }
            else
            {
                return new HTTPResponse<OrganisationTable, string>() { Success = true, HttpCode = 200, Data = org };
            }
        }

        public async Task<HTTPResponse<string, string>> RenameOrganisation(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please provide a new name" };
            }

            var organisation = await GetCurrentOrganisation();
            if (organisation == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "No organisation exists" };
            }

            organisation.Name = newName;
            try
            {
                await _organisationRepository.UpdateAsync(organisation);
                _logger.LogInformation($"Renamed organisation to {newName}");
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = $"Successfully renamed organisation to {organisation.Name}" };
            }
            catch (Exception)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to rename organisation" };
            }

        }

        public async Task<HTTPResponse<string, string>> UpdateOrganisationLogo(IFormFile logoFile)
        {
            if (logoFile == null || logoFile.Length == 0)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Please upload an image file" };
            }

            if (logoFile.Length > 2 * 1024 * 1024)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Logo file size must be 2MB or less" };
            }

            if (string.IsNullOrWhiteSpace(logoFile.ContentType) || !logoFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Only image files are allowed" };
            }

            var organisation = await GetCurrentOrganisation();
            if (organisation == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "No organisation exists" };
            }

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await logoFile.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            organisation.LogoImageData = fileBytes;
            organisation.LogoImageMimeType = logoFile.ContentType;

            try
            {
                await _organisationRepository.UpdateAsync(organisation);
                _logger.LogInformation("Updated organisation logo");
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully updated organisation logo" };
            }
            catch (Exception)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 500, Error = "Failed to update organisation logo" };
            }
        }

        private async Task<OrganisationTable?> GetCurrentOrganisation()
        {
            var organisations = await _organisationRepository.GetAll();
            return organisations.FirstOrDefault();
        }
    }
}
