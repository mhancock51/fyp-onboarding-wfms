using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IReadDocumentTaskTemplateRepository : IRepository<ReadDocumentTaskTemplateTable>
    {
        public Task<ReadDocumentTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId);
    }
    public class ReadDocumentTaskTemplateRepository : BaseRepository<ReadDocumentTaskTemplateTable>, IReadDocumentTaskTemplateRepository
    {
        public ReadDocumentTaskTemplateRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<ReadDocumentTaskTemplateTable> GetById(string id)
        {
            return await _dbContext.readDocumentTaskTemplates.FirstAsync(i => i.Id == id);            
        }

        public async Task<ReadDocumentTaskTemplateTable> GetByTaskTemplateId(string taskTemplateId)
        {
            return await _dbContext.readDocumentTaskTemplates.FirstAsync(i => i.TaskTemplateId == taskTemplateId);            
        }
    }

}
