using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateWorkflowTemplatePayload
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOnboardingWF { get; set; }
        public WorkflowTemplateNodeDTO[] PreflowTasks { get; set; }
        public WorkflowTemplateNodeDTO[] MainflowTasks { get; set; }

    }
}
