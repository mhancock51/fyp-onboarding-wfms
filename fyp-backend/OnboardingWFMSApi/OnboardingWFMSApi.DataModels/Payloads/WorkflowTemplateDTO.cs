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
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("isOnboardingWF")]
        public bool IsOnboardingWF { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }  
        [JsonInclude]
        [JsonPropertyName("preflowNodes")]
        public List<WorkflowTemplateNodeDTO> PreflowNodes { get; set; }
        [JsonInclude]
        [JsonPropertyName("mainflowNodes")]
        public List<WorkflowTemplateNodeDTO> MainflowNodes { get; set; }
        [JsonPropertyName("numberOfTasks")]
        public int NumberOfTasks
        {
            get { return PreflowNodes.Count + MainflowNodes.Count; }
        }
    }
}
