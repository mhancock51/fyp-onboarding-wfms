using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.BusinessLogic.Handlers.CustomAuthHandlers
{
    public class SupervisorUserRequirement : IAuthorizationRequirement
    {
    }

    public class SupervisorUserHandler : AuthorizationHandler<SupervisorUserRequirement>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<SupervisorUserHandler> _logger;

        public SupervisorUserHandler(IAccountRepository accountRepository, ILogger<SupervisorUserHandler> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, SupervisorUserRequirement requirement)
        {
            _logger.LogDebug("Handling supervisor user requirement...");
            // get account Id
            var accountId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                _logger.LogDebug("No account ID could be found in claims");
                context.Fail();
                return;
            }

            // fetch matching account
            var matchingAccount = await _accountRepository.GetById(accountId);
            if (matchingAccount == null)
            {
                _logger.LogDebug($"No account matching the ID {accountId} could be found");
                context.Fail();
                return;
            }
            // check account is active
            if (matchingAccount.AccountStatus != AccountConstants.REGISTERED_STATUS)
            {
                _logger.LogDebug($"Account status is not '{AccountConstants.REGISTERED_STATUS}': {matchingAccount.AccountStatus}");
                context.Fail();
                return;
            }
            // check is supervisor
            if (!matchingAccount.IsSupervisor)
            {
                _logger.LogDebug($"Account isn't a supervisor: {nameof(AccountTable.IsSupervisor)}={matchingAccount.IsSupervisor}");
                context.Fail();
                return;
            }

            context.Succeed(requirement);
            _logger.LogInformation("Supervisor user requirement met.");
            return;
        }
    }
}
