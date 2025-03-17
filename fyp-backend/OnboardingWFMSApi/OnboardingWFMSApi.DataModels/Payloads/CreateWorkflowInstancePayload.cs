using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateWorkflowInstancePayload
    {
        public string workflowTeamplateId { get; set; }
        public string? onboarderAccountId { get; set; }        
        public string supervisorAccountId { get; set; }
    }
}
