using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateTaskTemplatePayload
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatorAccountId { get; set; }
        public string TaskTypeId { get; set; }
        public object TaskTypeData { get; set; }
    }
}
