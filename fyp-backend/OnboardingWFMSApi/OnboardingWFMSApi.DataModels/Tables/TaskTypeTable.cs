using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("tasktype")]
    public class TaskTypeTable
    {
        [Key]
        public string TaskTypeId { get; set; }
        public string TaskName { get; set; }
    }
}
