using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Workflows
{
    [Table("workflowinstancenode")]
    public class WorkflowInstanceNodeTable : ITenantTableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(WorkflowInstanceTable.Id))]
        [JsonPropertyName("workflowInstanceId")]
        public string WorkflowInstanceId { get; set; }
        [ForeignKey(nameof(WorkflowTemplateTable.Id))]
        [JsonPropertyName("workflowTemplateId")]
        public string WorkflowTemplateId { get; set; }
        [ForeignKey(nameof(WorkflowTemplateNodeTable.Id))]
        [JsonPropertyName("workflowTemplateNodeId")]
        public string WorkflowTemplateNodeId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }

        public string TenantId { get; set; }
    }
}
