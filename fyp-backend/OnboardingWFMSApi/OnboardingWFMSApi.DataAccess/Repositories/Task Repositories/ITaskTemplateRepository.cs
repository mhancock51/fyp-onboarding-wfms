using OnboardingWFMSApi.DataModels;
using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface ITaskTemplateRepository<TEntity> : IRepository<TEntity> where TEntity : class, ITableEntity
    {
        public Task<TEntity> GetByTaskTemplateId(string id);
    }
}
