using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.Workflows;
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
    [Table("taskinstance")]
    public class TaskInstanceTable : ITenantTableEntity
    {
        [Key]
        [Column("TaskInstanceId")]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("assigneeAccountId")]
        public string AssigneeAccountId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("assignerAccountId")]
        public string AssignerAccountId { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [ForeignKey(nameof(WorkflowInstanceTable.Id))]
        [JsonPropertyName("workflowInstanceId")]
        public string? WorkflowInstanceId { get; set; }
        [JsonPropertyName("creationTimestamp")]
        public DateTime CreationTimestamp { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }
        [JsonPropertyName("workflowInstanceNodeId")]

        [ForeignKey(nameof(WorkflowInstanceNodeTable.Id))]
        public string? WorkflowInstanceNodeId { get; set; }

        [JsonPropertyName("completionTimestamp")]
        public DateTime? CompletionTimestamp { get; set; }

        public string TenantId { get; set; }

    }
}
