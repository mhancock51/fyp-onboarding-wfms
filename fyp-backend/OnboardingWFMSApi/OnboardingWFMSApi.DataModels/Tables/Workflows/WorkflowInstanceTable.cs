using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Workflows
{
    [Table("workflowinstance")]
    public class WorkflowInstanceTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [NotNull]
        [ForeignKey(nameof(WorkflowTemplateTable.Id))]
        public string WorkflowTemplateId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string SupervisorAccountId { get; set; }
        public DateTime CreationTimestamp { get; set; }                
        public DateTime? MainflowStartTimestamp { get; set; }
    }
}
