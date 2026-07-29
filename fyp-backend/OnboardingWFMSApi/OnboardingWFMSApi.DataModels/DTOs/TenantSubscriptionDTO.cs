using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingWFMSApi.DataModels.DTOs
{
    public class TenantSubscriptionDTO
    {
        public TenantSubscriptionDTO() { }

        public TenantSubscriptionDTO(DateTime createdDate, DateTime subscriptionCurrentPeriodEnd, SubscriptionTierDTO subscriptionTier, string stripeSubscriptionStatus)
        {
            CreatedDate = createdDate;
            SubscriptionCurrentPeriodEnd = subscriptionCurrentPeriodEnd;
            this.subscriptionTier = subscriptionTier;
            StripeSubscriptionStatus = stripeSubscriptionStatus;
        }

        public DateTime CreatedDate {get; set;}
        public DateTime SubscriptionCurrentPeriodEnd {get; set;}
        public SubscriptionTierDTO subscriptionTier {get; set;} = null!;
        public string StripeSubscriptionStatus {get; set;} = string.Empty;

    }

    public class SubscriptionTierDTO
    {
        public SubscriptionTierDTO(string displayName, bool canUploadDocuments, int maxActiveWorkflowInstances, int maxDocumentStorageSpaceInMb, int maxUsers)
        {
            DisplayName = displayName;
            CanUploadDocuments = canUploadDocuments;
            MaxActiveWorkflowInstances = maxActiveWorkflowInstances;
            MaxDocumentStorageSpaceInMb = maxDocumentStorageSpaceInMb;
            MaxUsers = maxUsers;
        }

        public string DisplayName { get; set; }
        public bool CanUploadDocuments {get; set;}
        public int MaxActiveWorkflowInstances {get; set;}
        public int MaxDocumentStorageSpaceInMb {get; set; }
        public int MaxUsers {get; set;}
    }
}