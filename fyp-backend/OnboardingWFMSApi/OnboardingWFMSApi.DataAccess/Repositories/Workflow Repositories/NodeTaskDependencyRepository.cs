using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataAccess.Repositories.Workflow_Repositories
{
    public interface INodeTaskDependencyRepository : IRepository<NodeTaskDependencyTable>
    {

    }
    public class NodeTaskDependencyRepository : BaseRepository<NodeTaskDependencyTable>, INodeTaskDependencyRepository
    {
        public NodeTaskDependencyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
