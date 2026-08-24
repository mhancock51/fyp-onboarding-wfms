using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Interfaces
{
    public interface ITaskTypeTemplateTable : ITenantTableEntity
    {
        public string TaskTemplateId { get; set; }
        public string TenantId { get; set; }
    }
}
