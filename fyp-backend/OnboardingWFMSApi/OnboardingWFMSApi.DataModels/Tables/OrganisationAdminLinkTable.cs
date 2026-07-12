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
    [Table("organisationAdminLink")]
    public class OrganisationAdminLinkTable : ITenantTableEntity
    {
        [Key]
        public string Id { get; set; }
        [ForeignKey(nameof(OrganisationTable.Id))]
        public string OrganisationId { get; set; }
        [ForeignKey(nameof(AccountTable.Id))]
        public string AccountId { get; set; }

        public string TenantId { get; set; }
    }
}
