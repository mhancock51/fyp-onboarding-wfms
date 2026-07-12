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
    [Table("readdocumenttasktemplate")]
    public class ReadDocumentTaskTemplateTable : ITaskTypeTemplateTable
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }
        [JsonPropertyName("documentUrl")]
        public string DocumentUrl { get; set; }
        [JsonPropertyName("checkBoxLabel")]
        public string CheckBoxLabel { get; set; }

        public string TenantId { get; set; }
    }
}
