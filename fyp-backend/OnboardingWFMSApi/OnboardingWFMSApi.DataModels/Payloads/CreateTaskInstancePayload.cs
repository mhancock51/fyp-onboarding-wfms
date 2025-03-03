using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateTaskInstancePayload
    {
        public string TaskTemplateId { get; set; }
        public string AssigneeAccountId { get; set; }
        public string AssignerAccountId { get; set; }
    }
}
