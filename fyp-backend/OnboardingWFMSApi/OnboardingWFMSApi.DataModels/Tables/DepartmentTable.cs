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
    public class DepartmentTable
    {
        [Key]
        public string DepartmentId { get; set; }
        public string DisplayName { get; set; }

    }
}
