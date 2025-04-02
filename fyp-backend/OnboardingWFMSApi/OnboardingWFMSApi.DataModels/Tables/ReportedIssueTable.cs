using Newtonsoft.Json;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("reportedissue")]
    public class ReportedIssueTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }       
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("issueCreatorId")]
        public string IssueCreatorId { get; set; }
        [JsonPropertyName("issueLoggedTimestamp")]
        public DateTime IssueLoggedTimestamp { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("suggestedChanges")]
        public string SuggestedChanges { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("remark")]
        public string? Remark { get; set; }
    }
}
