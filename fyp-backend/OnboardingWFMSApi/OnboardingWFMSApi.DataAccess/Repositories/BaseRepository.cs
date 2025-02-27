using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IRepository<TEntity>
    {
        public Task AddAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
    }

    public class BaseRepository<TEntity> : IRepository<TEntity>
    {
        protected readonly ApplicationDbContext _dbContext;

        public static string GenerateRandomId()
        {
            var random = new Random();
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var length = 8;
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = characters[random.Next(characters.Length)];
            }
            return new String(result);
        }

        public BaseRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;    
        }

        public virtual async Task AddAsync(TEntity entity)
        {            
            var result = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            var result = _dbContext.Remove(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            var result = _dbContext.Update(entity);
        }
    }
}
