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

        public async Task<List<CommentTable>> GetCommentsByTaskTemplateId(string taskTemplateId)
        {
            return await _dbContext.comments.Where(c => c.TaskTemplateId == taskTemplateId).ToListAsync();
        }
    }
}
