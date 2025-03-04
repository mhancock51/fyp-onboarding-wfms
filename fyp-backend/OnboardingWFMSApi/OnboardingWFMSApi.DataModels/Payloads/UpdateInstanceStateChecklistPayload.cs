using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateInstanceStateChecklistPayload
    {
        public string TaskInstanceId { get; set; }
        public bool[] ItemCompletionStatuses { get; set; }
    }
}
