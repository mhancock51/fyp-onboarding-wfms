using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("tasktemplate")]
    public class TaskTemplateTable : ITableEntity
    {
        [Key]
        [Column("TaskTemplateId")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string CreatorAccountId { get; set; }
        public DateTime DateCreated { get; set; }
        [ForeignKey(nameof(TaskTypeTable.Id))]
        public string TaskTypeId { get; set; }
    }
}
