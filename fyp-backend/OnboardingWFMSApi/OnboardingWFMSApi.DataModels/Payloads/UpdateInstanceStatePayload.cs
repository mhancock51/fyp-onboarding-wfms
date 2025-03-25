using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateInstanceStatePayload
    {
        [JsonPropertyName("updateTaskState")]
        public object UpdateTaskState { get; set; }
        [JsonPropertyName("taskTypeId")]
        public string TaskTypeId { get; set; }
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId { get; set; }        
    }
}
