using OnboardingWFMSApi.DataModels.Tables.Workflows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("onboardingemployeedetails")]
    public class OnboardingEmployeeDetailsTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(WorkflowInstanceTable.Id))]
        public string WorkflowInstanceId { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        [ForeignKey(nameof(DepartmentTable.Id))]
        public string DepartmentId { get; set; }
    }
}
