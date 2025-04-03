using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class CreateWorkflowInstanceAuditLogPayload
    {
        [JsonPropertyName("workflowInstanceId")]
        public string WorkflowInstanceId { get; set; }
        [JsonPropertyName("log")]
        public string Log { get; set; }
        [JsonPropertyName("accountId")]
        public string? AccountId { get; set; }

    }
}
