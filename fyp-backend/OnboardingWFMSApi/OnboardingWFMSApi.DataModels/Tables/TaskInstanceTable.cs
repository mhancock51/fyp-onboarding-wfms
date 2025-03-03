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
    public class TaskInstanceTable
    {
        [Key]
        public string TaskInstanceId { get; set; }
        [ForeignKey(nameof(AccountTable.AccountId))]
        public string AssigneeAccountId { get; set; }
        [ForeignKey(nameof(AccountTable.AccountId))]
        public string AssignerAccountId { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.TaskTemplateId))]
        public string TaskTemplateId { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string Status { get; set; }
    }
}
