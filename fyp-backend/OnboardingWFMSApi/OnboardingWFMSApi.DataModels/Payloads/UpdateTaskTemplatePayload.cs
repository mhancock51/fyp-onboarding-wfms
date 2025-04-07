using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateTaskTemplatePayload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("updatedDescription")]
        public string UpdatedDescription { get; set; }
        [JsonPropertyName("updateTaskTypeData")]
        public object? UpdateTaskTypeData { get; set; }
    }
}
