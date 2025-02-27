using Microsoft.EntityFrameworkCore;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories
{
    public interface IDepartmentRepository : IRepository<DepartmentTable>
    {
    }

    public class DepartmentRepository : BaseRepository<DepartmentTable>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task AddAsync(DepartmentTable entity)
        {            
            await base.AddAsync(entity);
        }

        public override async Task<bool> ExistsById(string id)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(i => i.DepartmentId == id) != null ? true : false;
        }
    }
}
