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
        // - Has an active subscription? - is customer paying?
        // - On hold -> flag to block users from this tennant from accessing the org/software


        [Key]
        public string Id { get; set;}
        public DateTime CreatedDateTime {get; set; }

        public string OwnerEmailAddress { get; set; }
        /// <summary>
        /// Flag for if tennant has an active subscription
        /// </summary>
        public bool HasActiveSubscription {get; set; }
        public bool IsOnHold {get; set;}

        [ForeignKey(nameof(SubscriptionTierEntitlementTable.Id))]
        public string SubscriptionTeirId { get; set; }
    }
}