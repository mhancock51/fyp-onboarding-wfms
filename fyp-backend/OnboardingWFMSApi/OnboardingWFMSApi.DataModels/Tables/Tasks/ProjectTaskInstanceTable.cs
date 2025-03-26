using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.Tasks
{
    [Table("projecttaskinstance")]
    public class ProjectTaskInstanceTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(TaskInstanceTable.Id))]
        public string TaskInstanceId { get; set; }
        public List<bool> ObjectiveStates { get; set; }
    }    
}
