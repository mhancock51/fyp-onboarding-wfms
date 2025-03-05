using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("checklisttasktemplate")]
    public class ChecklistTaskTemplateTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        public string TaskTemplateId { get; set; }
        public string[] Items { get; set; }
    }
}
