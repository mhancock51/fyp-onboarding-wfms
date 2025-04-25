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
    [Table("workflowinstanceauditlog")]
    public class WorkflowInstanceAuditLogTable : ITableEntity
    {
        public WorkflowInstanceAuditLogTable(string workflowInstanceId, string log, string? accountId)
        {
            Id = "";
            WorkflowInstanceId = workflowInstanceId;
            Log = log;
            Timestamp = DateTime.Now;
            AccountId = accountId;
        }

        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(WorkflowInstanceTable.Id))]
        [JsonPropertyName("workflowInstanceId")]
        public string WorkflowInstanceId {  get; set; }
        [JsonPropertyName("log")]
        public string Log {  get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string? AccountId { get; set; }  
    }
}
