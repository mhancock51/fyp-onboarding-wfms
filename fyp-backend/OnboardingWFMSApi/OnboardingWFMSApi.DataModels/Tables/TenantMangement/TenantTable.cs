using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;
using OnboardingWFMSApi.DataModels.Tables.TenantMangement;

namespace OnboardingWFMSApi.DataModels.Tables
{
    [Table("Tenants")]
    public class TenantTable : ITableEntity
    {
        // Per tentant:
        // - Id
        // - Email of owner of the organisation/subscription payer
        // - Created timestamp        

        [Key]
        public string Id { get; set;}
        public DateTime CreatedDateTime {get; set; }        

        [ForeignKey(nameof(AccountTable.Id))]
        public string OwnerAccountId {get; set;}
    }
}