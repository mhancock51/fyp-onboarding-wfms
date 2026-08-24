import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Separator } from '@/components/ui/separator';
import { SET_OPEN_CANCEL_SUBSCRIPTION_DIALOG, SET_OPEN_MANAGE_SUBSCRIPTION_DIALOG, SET_TENANT_SUBSCRIPTION } from '@/features/appSlice';
import HTTPresponse from '@/models/HTTPresponse';
import TenantSubscription from '@/models/TenantSubscription';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'sonner';
import { Loader2, Sparkles } from 'lucide-react';

export default function ManageSubscriptionDialog() {
  const dispatch = useDispatch();
  const open = useSelector((state: RootState) => state.app.openManageSubscriptionDialog);
  const tenantSubscription = useSelector((state: RootState) => state.app.tenantSubscription);
  const [loading, setLoading] = useState(false);

  function closeDialog() {
    dispatch(SET_OPEN_MANAGE_SUBSCRIPTION_DIALOG(false));
  }

  async function refreshSubscription() {
    setLoading(true);
    try {
      const response: AxiosResponse<HTTPresponse<TenantSubscription, string>> =
        await Api.tenantSubscription.fetchTenantSubscription();
      dispatch(SET_TENANT_SUBSCRIPTION(response.data.data));
    } catch {
      toast.error('Failed to fetch subscription details.');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (open) {
      refreshSubscription();
    }
  }, [open]);

  const formatDate = (dateStr: string): string => {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const statusVariant = (status: string): "default" | "destructive" | "outline" | "secondary" => {
    switch (status?.toLowerCase()) {
      case 'active':
      case 'trialing':
        return 'default';
      case 'past_due':
      case 'unpaid':
        return 'destructive';
      case 'canceled':
        return 'secondary';
      default:
        return 'outline';
    }
  };

  return (
    <Dialog open={open} onOpenChange={closeDialog}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Manage Subscription</DialogTitle>
          <DialogDescription>
            Review your current plan details and manage billing.
          </DialogDescription>
        </DialogHeader>

        {loading ? (
          <div className="flex items-center justify-center py-12">
            <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
          </div>
        ) : tenantSubscription ? (
          <div className="space-y-4 py-4">
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">Plan</span>
              <span className="text-sm font-semibold">{tenantSubscription.subscriptionTier.displayName}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">Status</span>
              <Badge variant={statusVariant(tenantSubscription.stripeSubscriptionStatus)}>
                {tenantSubscription.stripeSubscriptionStatus}
              </Badge>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">Started</span>
              <span className="text-sm">{formatDate(tenantSubscription.createdDate)}</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-sm text-muted-foreground">Current period ends</span>
              <span className="text-sm">{formatDate(tenantSubscription.subscriptionCurrentPeriodEnd)}</span>
            </div>

            <Separator />

            <div className="space-y-2">
              <h4 className="text-sm font-medium">Plan entitlements</h4>
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">Max active workflows</span>
                <span>{tenantSubscription.subscriptionTier.maxActiveWorkflowInstances}</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">Max users</span>
                <span>{tenantSubscription.subscriptionTier.maxUsers}</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">Document uploads</span>
                <span>{tenantSubscription.subscriptionTier.canUploadDocuments ? 'Yes' : 'No'}</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span className="text-muted-foreground">Document storage</span>
                <span>{tenantSubscription.subscriptionTier.maxDocumentStorageSpaceInMb} MB</span>
              </div>
            </div>
            <div className="flex items-center justify-center text-sm">
              <Button className='w-full cursor-not-allowed' variant={"secondary"} disabled={true}>
                Upgrade Tier
                <Sparkles className='mr-2 h-4 w-4' />
              </Button>
            </div>
          </div>
        ) : (
          <div className="py-8 text-center text-sm text-muted-foreground">
            No active subscription found.
          </div>
        )}

        <DialogFooter>
          <Button
            variant="destructive"
            disabled={tenantSubscription?.stripeSubscriptionStatus?.toLowerCase() === 'canceled'}
            onClick={() => {
              closeDialog();
              dispatch(SET_OPEN_CANCEL_SUBSCRIPTION_DIALOG(true));
            }}
          >
            Cancel Subscription
          </Button>
          <Button variant="outline" onClick={closeDialog}>
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
