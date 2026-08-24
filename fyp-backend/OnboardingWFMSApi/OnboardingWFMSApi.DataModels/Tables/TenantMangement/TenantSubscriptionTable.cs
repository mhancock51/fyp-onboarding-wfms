using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.TenantMangement
{
    [Table("TenantSubscriptions")]
    public class TenantSubscriptionTable : ITableEntity
    {
        [Key]
        public string Id {get; set;} 

        [ForeignKey(nameof(TenantTable.Id))]
        public string TenantId {get; set;}

        [ForeignKey(nameof(SubscriptionTierEntitlementTable.Id))]
        public string SubscriptionTierId {get; set;}
        public DateTime CreatedDate {get; set;}
        public string StripeCustomerId {get; set;}
        public string StripeSubscriptionId {get; set;}
        public string StripeSubscriptionStatus {get; set;}
        public DateTime StripeCurrentPeriodEnd {get; set;}
    }
}