using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IRepository<TEntity>
    {
        public Task<TEntity> AddAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task<TEntity> GetById(string id);
        public Task<bool> ExistsById(string id);
        public Task<List<TEntity>> GetAll();
    }

    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;    
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            // assign guid
            Guid guid = Guid.NewGuid();
            var guidStr = guid.ToString();

            var keyProp = typeof(TEntity).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute))).First();
            if (keyProp != null && keyProp.CanWrite)
            {
                keyProp.SetValue(entity, Convert.ChangeType(guidStr, keyProp.PropertyType), null);
            }
            else
            {
                throw new InvalidOperationException("Table datamodel doesn't have a key property");
            }

            var result = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            var result = _dbContext.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            var result = _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<TEntity> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> ExistsById(string id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            return _dbContext.Set<TEntity>().ToList();
        }
    }
}
