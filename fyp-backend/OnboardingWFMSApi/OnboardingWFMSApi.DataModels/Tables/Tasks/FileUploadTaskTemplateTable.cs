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
    [Table("fileuploadtasktemplate")]
    public class FileUploadTaskTemplateTable : ITaskTypeTemplateTable
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [JsonPropertyName("supportedDocumentType")]
        public string SupportedDocumentType { get; set; }
        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }
        [JsonPropertyName("accessAccountIds")]
        public string[] AccessAccountIds { get; set; }

        public string TenantId { get; set; }
    }
}
