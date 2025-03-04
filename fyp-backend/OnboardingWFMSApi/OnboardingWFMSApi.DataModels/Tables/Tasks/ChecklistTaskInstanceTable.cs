using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("checklisttaskinstance")]
    public class ChecklistTaskInstanceTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        public string TaskInstanceId { get; set; }
        /// <summary>
        /// bool value represent the completion status of the checklist item at the same index in the task template        
        /// </summary>
        public bool[] ItemCompletionStatuses { get; set; }
    }
}
