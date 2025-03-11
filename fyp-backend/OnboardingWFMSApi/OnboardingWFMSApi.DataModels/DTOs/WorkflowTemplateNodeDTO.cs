using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class WorkflowTemplateNodeDTO
    {
        public string TaskTemplateId { get; set; }
        public string AssigneeId { get; set; }
        public string[] DependencyTaskTemplateIds;
    }
}
