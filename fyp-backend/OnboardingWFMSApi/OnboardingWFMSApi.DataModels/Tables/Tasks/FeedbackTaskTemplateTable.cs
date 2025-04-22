using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("feedbacktasktemplate")]
    public class FeedbackTaskTemplateTable : ITaskTypeTemplateTable
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }

        [JsonPropertyName("questions")]
        public LikertQuestion[] Questions { get; set; }
       
    }

    public class LikertQuestion
    {
        [JsonPropertyName("question")]
        public string Question { get; set; }
        [JsonPropertyName("likertScale")]
        public string[] LikertScale { get; set; }
    }
}
