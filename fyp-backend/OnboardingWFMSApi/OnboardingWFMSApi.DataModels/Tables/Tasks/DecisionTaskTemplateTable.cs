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
    [Table("decisiontasktemplate")]
    public class DecisionTaskTemplateTable : ITaskTypeTemplateTable
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set;}
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        [JsonPropertyName("taskTemplateId")]
        public string TaskTemplateId { get; set; }
        [JsonPropertyName("question")]
        public string Question { get; set; }
        [JsonPropertyName("answerA")]
        public string AnswerA { get; set; }
        [JsonPropertyName("answerB")]
        public string AnswerB { get; set; }
        [JsonPropertyName("subflowId")]
        public string SubflowId { get; set; }
    }
}
