using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface ICommentRepository : IRepository<CommentTable>
    {
        public Task<List<CommentTable>> GetCommentsByTaskTemplateId(string taskTemplateId);
    }

    public class CommentRepository : BaseRepository<CommentTable>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<CommentTable> GetById(string id)
        {
            return await _dbContext.comments.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<CommentTable>> GetCommentsByTaskTemplateId(string taskTemplateId)
        {
            return await _dbContext.comments.Where(c => c.TaskTemplateId == taskTemplateId).ToListAsync();
        }
    }
}
