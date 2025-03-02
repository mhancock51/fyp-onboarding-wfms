using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("organisationAdminLink")]
    public class OrganisationAdminLinkTable
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(OrganisationTable.OrganisationId))]
        public string OrganisationId { get; set; }
        [ForeignKey(nameof(AccountTable.AccountId))]
        public string AccountId { get; set; }
    }
}
