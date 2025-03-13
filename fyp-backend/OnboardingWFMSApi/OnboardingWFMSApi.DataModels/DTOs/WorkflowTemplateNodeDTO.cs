using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class WorkflowTemplateNodeDTO
    {
        public string TaskTemplateId { get; set; }
        public string Id { get; set; }
        public string AssigneeId { get; set; }
        [JsonInclude]
        public List<string> DependencyTaskTemplateIds = new List<string>();
    }
}
