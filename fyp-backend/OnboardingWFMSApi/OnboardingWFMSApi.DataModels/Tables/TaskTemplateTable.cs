using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("tasktemplate")]
    public class TaskTemplateTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        [Column("TaskTemplateId")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("creatorAccountId")]
        public string CreatorAccountId { get; set; }
        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }
        [ForeignKey(nameof(TaskTypeTable.Id))]
        [JsonPropertyName("taskTypeId")]
        public string TaskTypeId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
