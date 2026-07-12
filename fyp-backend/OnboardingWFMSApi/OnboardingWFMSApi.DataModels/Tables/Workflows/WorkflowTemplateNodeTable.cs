using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.Workflows
{
    [Table("workflowtemplatenode")]
    public class WorkflowTemplateNodeTable : ITenantTableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(WorkflowTemplateTable.Id))]
        public string WorkflowTemplateId { get; set; }
        [ForeignKey(nameof(TaskTemplateTable.Id))]
        public string TaskTemplateId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string AssigneeId { get; set; }
        public int Order { get; set; }
        public string WorkflowSection {  get; set; }
        public int? DaysUntilDue { get; set; }
        public string[]? AccountsToNotify { get; set; }

        public string TenantId { get; set; }
    }
}
