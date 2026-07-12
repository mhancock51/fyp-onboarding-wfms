using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("tasktype")]
    public class TaskTypeTable : ITenantTableEntity
    {
        [Key]
        [Column("TaskTypeId")]
        public string Id { get; set; }
        public string TaskName { get; set; }

        public string TenantId { get; set; }
    }
}
