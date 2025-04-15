using AutoMapper;
using MediatR;
using OnboardingWFMSApi.BusinessLogic.MediatRHandlers;
using OnboardingWFMSApi.DataAccess.Repositories;
using OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories;
using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.DTOs;
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
        public Task<HTTPResponse<string, string>> InviteUser(string displayName, string emailAddress, string departmentId);
        public Task<HTTPResponse<string, string>> RegisterUser(string emailAddress, string hashedPassword, string hashedConfirmationPassword);
        public Task<HTTPResponse<InvitedAccountDTO, string>> GetInvitedAccount(string emailAddress);
        public Task<HTTPResponse<List<AccountDirectoryDTO>, string>> GetDirectoryOfAllRegisteredAccounts();
        public Task<AccountDirectoryDTO> GetDirectoryByAccountId(string accountId);
        public Task<bool> DoesAccountExistByEmail(string emailAddress);
        public Task<HTTPResponse<string, string>> MakeSupervisor(string accountId);
        public Task<HTTPResponse<string, string>> DeleteAccount(string accountId);
    }

    public class AccountLogic : IAccountLogic
    {
        public const string INVITED_STATUS = "invited";
        public const string REGISTERED_STATUS = "registered";
        public const string CLOSED_STATUS = "closed";

        private readonly IAccountRepository _accountRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountLogic(IAccountRepository accountRepository, IDepartmentRepository departmentRepository, IOrganisationRepository organisationRepository, 
            IMapper mapper, IMediator mediator, IWorkflowInstanceRepository workflowInstanceRepository)
        {
            _accountRepository = accountRepository;
            _departmentRepository = departmentRepository;
            _organisationRepository = organisationRepository;
            _mapper = mapper;
            _mediator = mediator;
            _workflowInstanceRepository = workflowInstanceRepository;
        }

        public async Task<HTTPResponse<string, string>> DeleteAccount(string accountId)
        {
            var account = await _accountRepository.GetById(accountId);
            if (account == null) return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Account doesn't exist" };
            // anonymise PI info
            account.EmailAddress = "---";
            account.DisplayName = "[Deleted]";
            account.AccountStatus = CLOSED_STATUS;

            await _accountRepository.UpdateAsync(account);


            return new HTTPResponse<string, string>() { Success = false, HttpCode = 200, Data = "Successfully deleted account" };
        }

        public async Task<bool> DoesAccountExistByEmail(string emailAddress)
        {
            return (await _accountRepository.GetByEmailAddress(emailAddress)) != null;
        }

        public async Task<AccountDirectoryDTO> GetDirectoryByAccountId(string accountId)
        {
            var account = await _accountRepository.GetById(accountId);
            if (account == null) return null;
            var dto = _mapper.Map<AccountDirectoryDTO>(account);
            // set department name
            dto.DepartmentName = (await _departmentRepository.GetById(account.DepartmentId)).DisplayName;
            return dto;
        }

        public async Task<HTTPResponse<List<AccountDirectoryDTO>, string>> GetDirectoryOfAllRegisteredAccounts()
        {
            // get all registed accounts
            var registeredAccounts = (await _accountRepository.GetAll()).Where(i => i.AccountStatus == REGISTERED_STATUS);
            var directory = new List<AccountDirectoryDTO>();
            foreach(var account in registeredAccounts)
            {
                var directoryItem = _mapper.Map<AccountDirectoryDTO>(account);
                // set department name
                directoryItem.DepartmentName = (await _departmentRepository.GetById(account.DepartmentId)).DisplayName;
                directory.Add(directoryItem);
            }
            return new HTTPResponse<List<AccountDirectoryDTO>, string>() { Success = true, HttpCode = 200, Data = directory };
        }

        public async Task<HTTPResponse<InvitedAccountDTO, string>> GetInvitedAccount(string emailAddress)
        {
            var account = await _accountRepository.GetByEmailAddress(emailAddress);
            if (account == null || account.AccountStatus != INVITED_STATUS)
            {
                return new HTTPResponse<InvitedAccountDTO, string>() { Success = false, HttpCode = 400, Error = "Invited account doesn't exist" };
            }
            else
            {
                // create DTO
                InvitedAccountDTO invitedAccount = _mapper.Map<InvitedAccountDTO>(account);
                DepartmentTable department = await _departmentRepository.GetById(account.DepartmentId);
                invitedAccount.DepartmentName = department.DisplayName;

                OrganisationTable organisation = await _organisationRepository.GetById(invitedAccount.OrganisationId);
                invitedAccount.OrganisationName = organisation.Name;

                return new HTTPResponse<InvitedAccountDTO, string>() { Success = true, HttpCode = 200, Data = invitedAccount };
            }
        }

        public async Task<HTTPResponse<string, string>> InviteUser(string displayName, string emailAddress, string departmentId)
        {           
            // check email address doesn't already exist
            var existingAccount = await _accountRepository.GetByEmailAddress(emailAddress);
            if (existingAccount != null)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account already exists", HttpCode = 400 };
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
                IsSupervisor = false,
                OrganisationId = "organisation",
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

        public async Task<HTTPResponse<string, string>> MakeSupervisor(string accountId)
        {
            // ensure account exists
            var existingAccount = await _accountRepository.GetById(accountId);
            if (existingAccount == null)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Account doesn't exist" };
            }
            // ensure account isn't already a supervisor
            if (existingAccount.IsSupervisor)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Account is already a supervisor" };
            }
            // ensure account is registered
            if (existingAccount.AccountStatus != REGISTERED_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Account isn't registered" };
            }
            // ensure account isn't onboarder of an open workflow instance
            // TODO: filter onboarding instances for ones that are open
            var onboardingInstances = await _workflowInstanceRepository.GetInstancesByOnboarderAccountId(accountId);
            if (onboardingInstances.Count > 0)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Account is an onboarder" };
            }
            // grant superviosor status
            existingAccount.IsSupervisor = true;
            try
            {
                await _accountRepository.UpdateAsync(existingAccount);
                return new HTTPResponse<string, string>() { Success = true, HttpCode = 200, Data = "Successfully granted supervisor status" };
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, HttpCode = 400, Error = "Failed to grant supervisor status" };
            }
        }

        public async Task<HTTPResponse<string, string>> RegisterUser(string emailAddress, string password, string confirmationPassword)
        {
            // hash both passwords
            password = AuthLogic.GetHashString(password);
            confirmationPassword = AuthLogic.GetHashString(confirmationPassword);


            // check account exists and status is set to invite
            var account = await _accountRepository.GetByEmailAddress(emailAddress);
            if (account == null)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account doesn't exist", HttpCode = 400 };
            }            
            if (account.AccountStatus != INVITED_STATUS)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Account is already registered", HttpCode = 400 };
            }

            // check hashed passwords match
            if (password != confirmationPassword)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Confirmation password doesn't match password", HttpCode = 400 };
            }
            
            // update record
            account.HashedPassword = password;
            account.AccountStatus  = REGISTERED_STATUS;

            // if first account, make admin
            if (await _accountRepository.GetNumberOfAccounts() == 0)
            {
                account.IsSupervisor = true;
            }

            try
            {
                await _accountRepository.UpdateAsync(account);
            }
            catch (Exception ex)
            {
                return new HTTPResponse<string, string>() { Success = false, Error = "Failed to register user", HttpCode = 500 };
            }
            await _mediator.Send(new AccountRegistrationRequest(account.Id, account.EmailAddress));
            // TODO: possibly implement check to fallback if failed from medaitors response

            return new HTTPResponse<string, string>() { Success = true, Data = "Registered user", HttpCode = 200 };
        }
    }
}
