using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateIssuePayload
    {
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("suggestedChanges")]
        public string SuggestedChanges { get; set; }
    }
}
