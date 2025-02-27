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

        public override async Task<AccountTable> GetById(string id)
        {
            return await _dbContext.Accounts.FirstOrDefaultAsync(e => e.AccountId == id);            
        }

        public override async Task<bool> ExistsById(string id)
        {
            return await _dbContext.Accounts.FirstOrDefaultAsync(i => i.AccountId == id) != null ? true : false;
        }

        public override async Task AddAsync(AccountTable entity)
        {
            // ensure no duplicate email address
            if (await GetByEmailAddress(entity.EmailAddress) != null)
            {
                throw new Exception("Account with email address already exists");
            }
            else
            {
                base.AddAsync(entity);
            }
        }

        public async Task<int> GetNumberOfAccounts()
        {
            return _dbContext.Accounts.Count();
        }
    }
}
