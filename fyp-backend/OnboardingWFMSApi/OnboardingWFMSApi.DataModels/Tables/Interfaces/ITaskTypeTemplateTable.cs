using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    public interface ITaskTypeTemplateTable : ITableEntity
    {
        public string TaskTemplateId { get; set; }
    }
}
