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
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("assigneeId")]
        public string AssigneeId { get; set; }
        [JsonPropertyName("dependencyNodeIds")]
        [JsonInclude]
        public List<string> DependencyNodeIds = new List<string>();
        [JsonPropertyName("daysUntilDue")]
        public int? DaysUntilDue { get; set; }
    }
}
