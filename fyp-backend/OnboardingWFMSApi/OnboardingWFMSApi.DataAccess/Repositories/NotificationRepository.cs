using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface INotificationRepository : IRepository<NotificationTable>
    {
        public Task<List<NotificationTable>> GetAccountsNotifications(string accountId);
    }

    public class NotificationRepository : BaseRepository<NotificationTable>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<NotificationTable>> GetAccountsNotifications(string accountId)
        {
            return await _dbContext.notifications.Where(n => n.RecipientId == accountId).ToListAsync();
        }
    }
}
