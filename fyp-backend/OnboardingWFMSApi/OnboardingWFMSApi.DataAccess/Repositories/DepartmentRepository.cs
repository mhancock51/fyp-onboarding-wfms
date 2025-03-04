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
        public Task<DepartmentTable> GetDepartmentByName(string name);
    }

    public class DepartmentRepository : BaseRepository<DepartmentTable>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<bool> ExistsById(string id)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(i => i.Id == id) != null ? true : false;
        }

        public override async Task<DepartmentTable> GetById(string id)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(i => i.Id == id);            
        }

        public async Task<DepartmentTable> GetDepartmentByName(string name)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(i => i.DisplayName.ToLower() == name);
        }
    }
}
