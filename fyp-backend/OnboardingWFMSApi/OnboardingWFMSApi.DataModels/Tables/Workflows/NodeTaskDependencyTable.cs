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
    [Table("nodetaskdependency")]
    public class NodeTaskDependencyTable : ITenantTableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(WorkflowTemplateNodeTable.Id))]
        public string NodeId { get; set; }
        [ForeignKey(nameof(WorkflowTemplateNodeTable.Id))]
        public string DependencyNodeId { get; set; }
        [ForeignKey(nameof(WorkflowTemplateTable.Id))]
        public string WorkflowTemplateId { get; set; }

        public string TenantId { get; set; }
    }
}
