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
    [Table("checklisttaskinstance")]
    public class ChecklistTaskInstanceTable : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        [JsonPropertyName("taskInstanceId")]
        public string TaskInstanceId { get; set; }
        /// <summary>
        /// bool value represent the completion status of the checklist item at the same index in the task template        
        /// </summary>
        [JsonPropertyName("itemCompletionStatuses")]
        public bool[] ItemCompletionStatuses { get; set; }
    }
}
