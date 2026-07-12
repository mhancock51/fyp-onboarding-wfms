using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;

namespace OnboardingWFMSApi.BusinessLogic.TenantLogic
{
    public interface ITenantOnboardingLogic
    {
        public Task<HTTPResponse<string, string>> InitialiseTenant(CreateTenantPayload payload);
    }

    public class TenantBoardingLogic : ITenantOnboardingLogic
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<TenantBoardingLogic> _logger;

        public TenantBoardingLogic(ITenantRepository tenantRepository, ILogger<TenantBoardingLogic> logger)
        {
            _tenantRepository = tenantRepository;
            _logger = logger;
        }

        /// <summary>
        /// responsible for initial setup of tenant and creation of organisation, doesn't activate the tenant so access can't be made until subscription is called
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task<HTTPResponse<string, string>> InitialiseTenant(CreateTenantPayload payload)
        {
            try
            {
                await _tenantRepository.AddAsync(new TenantTable
                {
                    CreatedDateTime = DateTime.Now,
                    OwnerEmailAddress = payload.OwnerEmailAddress,
                    HasActiveSubscription = false,
                    IsOnHold = false,
                    SubscriptionTeirId = payload.SubscriptionTeirId
                });
                return new HTTPResponse<string, string>() { Success = true, Message = "Successfully created tenant", HttpCode = 200 };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create tenant: {ex}");
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to create tenant", HttpCode = 500 };
            }
        }
    }
}