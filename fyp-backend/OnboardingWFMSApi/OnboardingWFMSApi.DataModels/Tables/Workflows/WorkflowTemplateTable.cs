using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables.Workflows
{
    [Table("workflowtemplate")]
    public class WorkflowTemplateTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        public bool IsOnboardingWF { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
