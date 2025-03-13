using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class WorkflowTemplateDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOnboardingWF { get; set; }
        [JsonInclude]
        public List<WorkflowTemplateNodeDTO> PreflowTasks { get; set; }
        [JsonInclude]
        public List<WorkflowTemplateNodeDTO> MainflowTasks { get; set; }
    }
}
