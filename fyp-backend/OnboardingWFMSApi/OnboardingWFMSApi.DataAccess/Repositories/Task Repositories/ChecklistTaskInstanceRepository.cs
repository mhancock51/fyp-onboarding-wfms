using OnboardingWFMSApi.DataModels.Tables.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Task_Repositories
{
    public interface IChecklistTaskInstanceRepository : IRepository<ChecklistTaskInstanceTable>
    {

    }
    public class ChecklistTaskInstanceRepository : BaseRepository<ChecklistTaskInstanceTable>, IChecklistTaskInstanceRepository
    {
        public ChecklistTaskInstanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }

}
