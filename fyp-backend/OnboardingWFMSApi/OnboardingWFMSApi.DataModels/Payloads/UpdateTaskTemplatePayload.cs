using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateTaskTemplatePayload
    {
        public string Id { get; set; }
        public string UpdatedDescription { get; set; }
        public object UpdateTaskTypeData { get; set; }
    }
}
