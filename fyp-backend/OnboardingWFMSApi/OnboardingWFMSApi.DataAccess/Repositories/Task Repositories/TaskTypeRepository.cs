using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface ITaskTypeRepository : IRepository<TaskTypeTable>
    {
    }
    public class TaskTypeRepository : BaseRepository<TaskTypeTable>, ITaskTypeRepository
    {
        public TaskTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }

}
