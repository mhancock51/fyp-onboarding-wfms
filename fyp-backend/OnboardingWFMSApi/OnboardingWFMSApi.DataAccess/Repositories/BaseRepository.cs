using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, ITableEntity
    {
        public Task<TEntity> AddAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task<TEntity> GetById(string id);
        public Task<bool> ExistsById(string id);
        public Task<List<TEntity>> GetAll();
        public Task<List<TEntity>> AddManyAsync(List<TEntity> entities);
    }

    

    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class, ITableEntity
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
                // ensure id hasn't already been set
                var value = keyProp.GetValue(entity);
                if (value == null || (value is string str && str == ""))
                {
                    keyProp.SetValue(entity, Convert.ChangeType(guidStr, keyProp.PropertyType), null);
                }
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
            // prevent duplicate tracking
            var existingEntity = _dbContext.Set<TEntity>().Local
            .FirstOrDefault(e => e == entity || e.Id == entity.Id); // Assuming 'Id' is the primary key

            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _dbContext.Update(entity);
            }

            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<TEntity> GetById(string id)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(e => e.Id == id);            
        }

        public virtual async Task<bool> ExistsById(string id)
        {
            var entity = _dbContext.Set<TEntity>().Local.FirstOrDefault(e => e.Id == id);
            return entity != null ? true : false;
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        public virtual async Task<List<TEntity>> AddManyAsync(List<TEntity> entities)
        {
            var savedEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                var result = await AddAsync(entity);
                savedEntities.Add(result);
            }

            var rows = await _dbContext.SaveChangesAsync();

            return savedEntities;
        }
    }
}
