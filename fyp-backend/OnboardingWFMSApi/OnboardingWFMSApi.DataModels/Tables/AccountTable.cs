using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("account")]
    public class AccountTable : ITenantTableEntity
    {
        [Key]
        [Column("AccountId")]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string HashedPassword { get; set; }        
        public bool IsSupervisor { get; set; }
        [ForeignKey(nameof(DepartmentTable.Id))]
        public string DepartmentId { get; set; }
        [ForeignKey(nameof(OrganisationTable.Id))]
        public string OrganisationId { get; set; }
        public string AccountStatus { get; set; }
        public string TenantId { get; set;}
    }
}
