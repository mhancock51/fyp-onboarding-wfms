using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("department")]
    public class DepartmentTable : ITenantTableEntity
    {
        [Key]
        [Column("DepartmentId")]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string TenantId { get; set; }

    }
}
