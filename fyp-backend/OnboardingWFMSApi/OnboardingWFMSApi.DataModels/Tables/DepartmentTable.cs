using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("department")]
    public class DepartmentTable : ITableEntity
    {
        [Key]
        [Column("DepartmentId")]
        public string Id { get; set; }
        public string DisplayName { get; set; }

    }
}
