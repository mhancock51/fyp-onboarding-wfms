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
    [Table("readdocumenttaskinstance")]
    public class ReadDocumentTaskInstanceTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId { get; set; }
        [JsonPropertyName("checkboxChecked")]
        public bool CheckboxChecked { get; set; }
        [JsonPropertyName("linkClicked")]
        public bool LinkClicked { get; set; }
    }
}
