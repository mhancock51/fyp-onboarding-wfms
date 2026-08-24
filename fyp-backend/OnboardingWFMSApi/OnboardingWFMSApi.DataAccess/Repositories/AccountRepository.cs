using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IAccountRepository : IRepository<AccountTable>
    {
        public Task<AccountTable> GetByEmailAddress(string emailAddress);
        public Task<int> GetNumberOfAccounts();
        public Task<bool> IsAccountRegistered(string accountId);
        public Task<bool> DoesAccountExistByEmail(string emailAddress);
    }

    public class AccountRepository : BaseRepository<AccountTable>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<AccountTable> GetByEmailAddress(string emailAddress)
        {
            return await _dbContext.Accounts.FirstOrDefaultAsync(i => i.EmailAddress == emailAddress);
        }

        public override async Task<AccountTable> AddAsync(AccountTable entity)
        {
            // ensure no duplicate email address
            if (await GetByEmailAddress(entity.EmailAddress) != null)
            {
                throw new Exception("Account with email address already exists");
            }
            else
            {
                return await base.AddAsync(entity);
            }
        }

        public async Task<int> GetNumberOfAccounts()
        {
            return _dbContext.Accounts.Count();
        }

        public async Task<bool> IsAccountRegistered(string accountId)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return false;
            if (account.AccountStatus == "registered")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DoesAccountExistByEmail(string emailAddress)
        {
            return (await GetByEmailAddress(emailAddress)) != null;
        }
    }
}
