using Newtonsoft.Json;
using OnboardingWFMSApi.DataModels.Models;
using OnboardingWFMSApi.DataModels.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class IssueDTO : ReportedIssueTable
    {
        [JsonPropertyName("issueCreatorAccount")]
        public AccountDirectoryDTO IssueCreatorAccount { get; set; }
        [JsonPropertyName("taskInstance")]
        public TaskInstanceDTO TaskInstance { get; set; }
    }
}
