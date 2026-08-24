using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.BusinessLogic.StripeLogic;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Tenant_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
using OnboardingWFMSApi.DataModels.Payloads;
using OnboardingWFMSApi.DataModels.Tables;

namespace OnboardingWFMSApi.BusinessLogic
{
    public interface IGetStartedLogic
    {
        Task<HTTPResponse<GetStartedResponseDTO, string>> GetStarted(GetStartedPayload payload);
    }

    public class GetStartedLogic : IGetStartedLogic
    {
        private readonly ILogger<GetStartedLogic> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISubscriptionTierRepository _subscriptionTierRepository;
        private readonly IStripeLogic _stripeLogic;

        private readonly string _issuer;
        private readonly SymmetricSecurityKey _key;
        private readonly string _audience;
        private readonly double _tokenLifespan;

        public GetStartedLogic(
            ILogger<GetStartedLogic> logger,
            IConfiguration configuration,
            IAccountRepository accountRepository,
            ITenantRepository tenantRepository,
            IOrganisationRepository organisationRepository,
            IDepartmentRepository departmentRepository,
            ISubscriptionTierRepository subscriptionTierRepository,
            IStripeLogic stripeLogic)
        {
            _logger = logger;
            _configuration = configuration;
            _accountRepository = accountRepository;
            _tenantRepository = tenantRepository;
            _organisationRepository = organisationRepository;
            _departmentRepository = departmentRepository;
            _subscriptionTierRepository = subscriptionTierRepository;
            _stripeLogic = stripeLogic;

            _issuer = _configuration["Auth:Issuer"] ?? string.Empty;
            string keyStr = _configuration["Auth:Key"] ?? string.Empty;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            _audience = _configuration["Auth:Audience"] ?? string.Empty;
            _tokenLifespan = double.Parse(_configuration["Auth:TokenLifespanInMinutes"] ?? "1440");
        }

        public async Task<HTTPResponse<GetStartedResponseDTO, string>> GetStarted(GetStartedPayload payload)
        {
            if (string.IsNullOrWhiteSpace(payload.DisplayName))
                return Error(400, "Display name is required.");
            if (string.IsNullOrWhiteSpace(payload.Email))
                return Error(400, "Email is required.");
            if (string.IsNullOrWhiteSpace(payload.Password) || payload.Password.Length < 6)
                return Error(400, "Password must be at least 6 characters.");
            if (string.IsNullOrWhiteSpace(payload.OrganisationName))
                return Error(400, "Organisation name is required.");
            if (string.IsNullOrWhiteSpace(payload.SubscriptionTierId))
                return Error(400, "Subscription tier is required.");

            if (await _accountRepository.DoesAccountExistByEmail(payload.Email))
                return Error(400, "An account with this email already exists.");

            var tier = await _subscriptionTierRepository.GetById(payload.SubscriptionTierId);
            if (tier == null)
                return Error(400, "Selected subscription tier does not exist.");
            if (!tier.IsActive)
                return Error(400, "Selected subscription tier is not currently available.");

            try
            {
                // Step 1: Create tenant first
                var tenant = await _tenantRepository.AddAsync(new TenantTable
                {
                    CreatedDateTime = DateTime.UtcNow,
                    OwnerAccountId = string.Empty
                });
                _logger.LogInformation("Created tenant {TenantId}", tenant.Id);

                // Step 2: Create admin account (TenantId pre-set)
                var account = new AccountTable
                {
                    DisplayName = payload.DisplayName,
                    EmailAddress = payload.Email,
                    HashedPassword = AuthLogic.GetHashString(payload.Password),
                    IsSupervisor = true,
                    AccountStatus = AccountConstants.REGISTERED_STATUS,
                    DepartmentId = string.Empty,
                    OrganisationId = string.Empty,
                    TenantId = tenant.Id
                };

                account = await _accountRepository.AddAsync(account);
                _logger.LogInformation("Created admin account {AccountId} ({Email})", account.Id, account.EmailAddress);

                tenant.OwnerAccountId = account.Id;
                await _tenantRepository.UpdateAsync(tenant);

                // Step 3: Create organisation
                var organisation = await _organisationRepository.AddAsync(new OrganisationTable
                {
                    Name = payload.OrganisationName,
                    TenantId = tenant.Id
                });
                _logger.LogInformation("Created organisation {OrgId} ({OrgName}) for tenant {TenantId}",
                    organisation.Id, organisation.Name, tenant.Id);

                account.OrganisationId = organisation.Id;
                await _accountRepository.UpdateAsync(account);

                // Step 4: Create default "General" department
                var defaultDepartment = await _departmentRepository.AddAsync(new DepartmentTable
                {
                    DisplayName = "General",
                    TenantId = tenant.Id
                });
                _logger.LogInformation("Created default department {DeptId} (General) for tenant {TenantId}",
                    defaultDepartment.Id, tenant.Id);

                account.DepartmentId = defaultDepartment.Id;
                await _accountRepository.UpdateAsync(account);

                // Step 5: Create Stripe checkout session
                var checkoutResponse = await _stripeLogic.CreateCheckoutSession(tenant.Id, tier.Id);
                if (!checkoutResponse.Success || checkoutResponse.Data == null)
                {
                    _logger.LogError("Stripe checkout creation failed for tenant {TenantId}: {Error}",
                        tenant.Id, checkoutResponse.Error);
                    return Error(checkoutResponse.HttpCode,
                        checkoutResponse.Error ?? "Failed to create Stripe checkout session.");
                }

                var session = checkoutResponse.Data;

                // Step 6: Generate JWT
                var jwtToken = GenerateToken(account);

                var responseDto = new GetStartedResponseDTO
                {
                    CheckoutUrl = session.Url,
                    JwtToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    DisplayName = account.DisplayName,
                    Email = account.EmailAddress,
                    TenantId = tenant.Id,
                    TierName = tier.DisplayName
                };

                _logger.LogInformation("Get-started flow completed for {Email}, tenant {TenantId}, checkout {SessionId}",
                    payload.Email, tenant.Id, session.Id);

                return new HTTPResponse<GetStartedResponseDTO, string>
                {
                    Success = true,
                    HttpCode = 200,
                    Data = responseDto,
                    Message = "Account created. Complete payment to activate your subscription."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get-started flow failed for {Email}: {Message}", payload.Email, ex.Message);
                return Error(500, "An unexpected error occurred during sign-up. Please try again.");
            }
        }

        private JwtSecurityToken GenerateToken(AccountTable account)
        {
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id),
                new Claim("TenantId", account.TenantId)
            };

            return new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenLifespan),
                signingCredentials: creds
            );
        }

        private static HTTPResponse<GetStartedResponseDTO, string> Error(int httpCode, string message)
        {
            return new HTTPResponse<GetStartedResponseDTO, string>
            {
                Success = false,
                HttpCode = httpCode,
                Error = message
            };
        }
    }
}
