using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("taskinstance")]
    public class TaskInstanceTable : ITableEntity
    {
        [Key]
        [Column("TaskInstanceId")]
        public string Id { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string AssigneeAccountId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string AssignerAccountId { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]        
        public string TaskTemplateId { get; set; }
        [ForeignKey(nameof(WorkflowInstanceTable.Id))]
        public string? WorkflowInstanceId { get; set; } 
        public DateTime CreationTimestamp { get; set; }
        public string Status { get; set; }
    }
}
