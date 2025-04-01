using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("fileuploadtaskinstance")]
    public class FileUploadTaskInstanceTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId { get; set; }
        [ForeignKey(nameof(DocumentTable.Id))]
        [JsonPropertyName("DocumentId")]
        public string DocumentId { get; set; }
    }
}
