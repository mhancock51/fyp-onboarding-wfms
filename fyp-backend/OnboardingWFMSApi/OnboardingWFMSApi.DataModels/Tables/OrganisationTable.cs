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
    [Table("organisation")]
    public class OrganisationTable : ITenantTableEntity
    {
     
        [Key]
        [Column("OrganisationId")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
    }
}
