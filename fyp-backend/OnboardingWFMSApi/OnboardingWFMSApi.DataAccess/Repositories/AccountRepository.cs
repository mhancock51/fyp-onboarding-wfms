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
        
    }

    public class AccountRepository : BaseRepository<AccountTable>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<AccountTable> GetById(string id)
        {
            return await _dbContext.Accounts.FirstOrDefaultAsync(e => e.AccountId == id);            
        }
    }
}
