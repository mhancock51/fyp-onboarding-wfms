using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using OnboardingWFMSApi.DataModels.Tables.Interfaces;

namespace OnboardingWFMSApi.DataModels.Tables.TenantMangement
{
    [Table("SubscriptionTierEntitlements")]
    public class SubscriptionTierEntitlementTable : ITableEntity
    {
        [Key]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool CanUploadDocuments {get; set;}
        /// <summary>
        /// Max number of active workflows a tenant can have at any time
        /// </summary>
        public int MaxActiveWorkflowInstances {get; set;}
        public int MaxDocumentStorageSpaceInMb {get; set; }
        public int MaxUsers {get; set;}
        public DateTime CreatedDate {get; set;}
        /// <summary>
        /// Is available to tenants
        /// </summary>
        public bool IsActive {get; set; }

        public string PriceId {get; set;}
    }
}