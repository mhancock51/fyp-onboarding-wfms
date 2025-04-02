using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Payloads
{
    public class UpdateIssueStatusPayload
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("remark")]
        public string Remark { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
    }
}
