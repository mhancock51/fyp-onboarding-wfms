using OnboardingWFMSApi.DataModels.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateWorkflowTemplatePayload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("isOnboardingWF")]
        public bool IsOnboardingWF { get; set; }
        [JsonInclude]
        [JsonPropertyName("preflowNodes")]
        public List<CreateWorkflowTemplateNode> PreflowNodes { get; set; }
        [JsonInclude]
        [JsonPropertyName("mainflowNodes")]
        public List<CreateWorkflowTemplateNode> MainflowNodes { get; set; }
    }

    public class CreateWorkflowTemplateNode
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
        [JsonPropertyName("accountsToNotify")]
        public string[] AccountsToNotify { get; set; }

    }
}
