using OnboardingWFMSApi.DataModels.Tables.Interfaces;
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
    [Table("notification")]
    public class NotificationTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id {  get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        [JsonPropertyName("recipientId")]
        public string RecipientId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
