using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.Workflows
{
    [Table("workflowinstance")]
    public class WorkflowInstanceTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [NotNull]
        [ForeignKey(nameof(WorkflowTemplateTable.Id))]
        [JsonPropertyName("workflowTemplateId")]
        public string WorkflowTemplateId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("supervisorAccountId")]
        public string SupervisorAccountId { get; set; }
        [JsonPropertyName("creationTimestamp")]
        public DateTime CreationTimestamp { get; set; }
        [JsonPropertyName("mainflowStartTimestamp")]
        public DateTime? MainflowStartTimestamp { get; set; }
        [JsonPropertyName("completionTimestamp")]
        public DateTime? CompletionTimestamp { get; set; }
    }
}
