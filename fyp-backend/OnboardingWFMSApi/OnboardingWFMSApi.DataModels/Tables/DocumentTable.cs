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
    [Table("document")]
    public class DocumentTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId {  get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("creatorId")]
        public string CreatorId { get; set; }        
        [ForeignKey(nameof(WorkflowInstanceTable.Id))]
        [JsonPropertyName("workflowInstanceId")]
        public string? WorkflowInstanceId {  get; set; }
        [JsonPropertyName("documentData")]
        public byte[] DocumentData { get; set; }
        [JsonPropertyName("fileExtension")]
        public string FileExtension { get; set; }
        [JsonPropertyName("uploadTimestamp")]
        public DateTime UploadTimestamp { get; set; }
        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
    }
}
