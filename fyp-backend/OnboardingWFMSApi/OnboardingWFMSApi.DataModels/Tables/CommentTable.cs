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
    [Table("comment")]
    public class CommentTable : ITableEntity
    {
        [JsonPropertyName("id")]
        [Key]
        public string Id {  get; set; }
        [JsonPropertyName("commentorId")]
        [ForeignKey(nameof(AccountTable.Id))]
        public string CommenterId { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("taskTemplateId")]
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        public string? TaskTemplateId { get; set; }
        [JsonPropertyName("creationTimestamp")]
        public DateTime CreationTimestamp { get; set; } 
    }
}
