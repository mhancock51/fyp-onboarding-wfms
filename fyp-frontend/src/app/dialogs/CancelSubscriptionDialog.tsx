import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { SET_OPEN_CANCEL_SUBSCRIPTION_DIALOG, SET_TENANT_SUBSCRIPTION } from '@/features/appSlice';
import HTTPresponse from '@/models/HTTPresponse';
import TenantSubscription from '@/models/TenantSubscription';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'sonner';
import { Loader2, AlertTriangle } from 'lucide-react';

export default function CancelSubscriptionDialog() {
  const dispatch = useDispatch();
  const open = useSelector((state: RootState) => state.app.openCancelSubscriptionDialog);
  const organisation = useSelector((state: RootState) => state.app.organisation);

  const [confirmText, setConfirmText] = useState('');
  const [loading, setLoading] = useState(false);

  const requiredText = organisation?.name ?? '';

  const isConfirmed = confirmText === requiredText && requiredText.length > 0;

  function closeDialog() {
    dispatch(SET_OPEN_CANCEL_SUBSCRIPTION_DIALOG(false));
    setConfirmText('');
  }

  useEffect(() => {
    if (open) {
      setConfirmText('');
    }
  }, [open]);

  async function handleCancel() {
    if (!isConfirmed) return;
    setLoading(true);
    try {
      const response: AxiosResponse<HTTPresponse<string, string>> =
        await Api.tenantSubscription.cancelTenantSubscription();
      if (response.data.success) {
        toast.success(response.data.message || 'Subscription cancelled successfully.');
        // Refresh the subscription data
        const subResponse: AxiosResponse<HTTPresponse<TenantSubscription, string>> =
          await Api.tenantSubscription.fetchTenantSubscription();
        dispatch(SET_TENANT_SUBSCRIPTION(subResponse.data.data));
      } else {
        toast.error(response.data.error || 'Failed to cancel subscription.');
      }
    } catch {
      toast.error('Failed to cancel subscription. Please try again.');
    } finally {
      setLoading(false);
      closeDialog();
    }
  }

  return (
    <Dialog open={open} onOpenChange={closeDialog}>
      <DialogContent className="sm:max-w-[480px]">
        <DialogHeader>
          <div className="flex items-center gap-2 text-destructive">
            <AlertTriangle className="h-5 w-5" />
            <DialogTitle>Cancel Subscription</DialogTitle>
          </div>
          <DialogDescription>
            This action <strong>cannot be undone</strong>. Your subscription will remain active
            until the end of the current billing period, after which all paid features will be
            revoked.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-3 py-4">
          <p className="text-sm text-muted-foreground">
            To confirm cancellation, type your organisation name below:
          </p>
          <div className="rounded-md border bg-muted/50 px-3 py-2">
            <p className="text-sm font-semibold select-all">{requiredText}</p>
          </div>

          <div className="space-y-1.5">
            <Label htmlFor="confirm-cancel-input" className="text-sm">
              Organisation name
            </Label>
            <Input
              id="confirm-cancel-input"
              placeholder={requiredText}
              value={confirmText}
              onChange={(e) => setConfirmText(e.target.value)}
              disabled={loading}
              autoComplete="off"
            />
            {confirmText.length > 0 && !isConfirmed && (
              <p className="text-xs text-destructive">
                The organisation name does not match.
              </p>
            )}
          </div>
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={closeDialog} disabled={loading}>
            Keep Subscription
          </Button>
          <Button
            variant="destructive"
            onClick={handleCancel}
            disabled={!isConfirmed || loading}
          >
            {loading ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Cancelling…
              </>
            ) : (
              'Confirm Cancellation'
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
