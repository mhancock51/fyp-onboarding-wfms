using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class ReportedIssuesAnalyticsDTO
    {
        [JsonPropertyName("openTaskIssues")]
        public int OpenTaskIssues { get; set; }
    }
}
