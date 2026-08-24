export interface SubscriptionTier {
  displayName: string;
  canUploadDocuments: boolean;
  maxActiveWorkflowInstances: number;
  maxDocumentStorageSpaceInMb: number;
  maxUsers: number;
}

export default interface TenantSubscription {
  createdDate: string;
  subscriptionCurrentPeriodEnd: string;
  stripeSubscriptionStatus: string;
  subscriptionTier: SubscriptionTier;
}
