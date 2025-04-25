using Microsoft.AspNetCore.Authorization;
using OnboardingWFMSApi.BusinessLogic.AccountLogic;
using OnboardingWFMSApi.DataAccess.Repositories;
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

        public SupervisorUserHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, SupervisorUserRequirement requirement)
        {
            // get account Id
            var accountId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
            {
                context.Fail();
                return;
            }

            // fetch matching account
            var matchingAccount = await _accountRepository.GetById(accountId);
            if (matchingAccount == null)
            {
                context.Fail();
                return;
            }
            // check account is active
            if (matchingAccount.AccountStatus != AccountConstants.REGISTERED_STATUS)
            {
                context.Fail();
                return;
            }
            // check is supervisor
            if (!matchingAccount.IsSupervisor)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
            return;
        }
    }
}
