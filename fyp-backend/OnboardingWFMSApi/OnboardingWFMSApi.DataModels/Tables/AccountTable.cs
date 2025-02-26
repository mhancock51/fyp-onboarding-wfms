using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("account")]
    public class AccountTable
    {
        [Key]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public string HashedPassword { get; set; }
        public bool IsOnboarder { get; set; }
        public bool IsAdmin { get; set; }
        public string DepartmentId { get; set; }
        public string OrganisationId { get; set; }
    }
}
