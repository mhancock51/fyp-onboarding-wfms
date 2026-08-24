import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { RootState } from '@/store';
import { useState } from 'react';
import { useSelector } from 'react-redux';
import { toast } from 'sonner';

type StripeCheckoutSession = {
  id?: string;
  url?: string;
  customer?: string | null;
  subscription?: string | null;
  metadata?: Record<string, string>;
};

export default function StripeCheckoutTestPage() {
  const user = useSelector((state: RootState) => state.app.user);
  const [subscriptionTierId, setSubscriptionTierId] = useState('');
  const [loading, setLoading] = useState(false);
  const [session, setSession] = useState<StripeCheckoutSession | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function createCheckoutSession() {
    if (!subscriptionTierId.trim()) {
      toast.error('Enter a subscription tier id first.');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const response = await Api.stripe.createCheckoutSession(subscriptionTierId.trim());
      const checkoutSession = response.data.data as StripeCheckoutSession;

      if (!checkoutSession?.url) {
        throw new Error('Stripe did not return a checkout URL.');
      }

      setSession(checkoutSession);
      toast.success('Checkout session created.');
    } catch (requestError: any) {
      const message = requestError?.response?.data?.error ?? requestError?.response?.data?.message ?? requestError?.message ?? 'Failed to create checkout session.';
      setError(message);
      setSession(null);
      toast.error(message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex flex-col gap-4 w-full max-w-3xl mx-auto">
      <div>
        <h1 className="text-2xl font-bold text-foreground">Stripe checkout test</h1>
        <p className="text-sm text-muted-foreground">Create a checkout session for the current tenant and inspect the returned Stripe session.</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Checkout session</CardTitle>
          <CardDescription>
            This calls <span className="font-medium">POST /api/stripe/create-checkout-session</span> using the logged-in tenant context.
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4">
          <div className="grid gap-2">
            <label className="text-sm font-medium text-foreground" htmlFor="subscription-tier-id">Subscription tier id</label>
            <Input
              id="subscription-tier-id"
              value={subscriptionTierId}
              onChange={(event) => setSubscriptionTierId(event.target.value)}
              placeholder="Paste a Stripe subscription tier id"
            />
          </div>

          <div className="text-sm text-muted-foreground">
            <div>Tenant: {user?.tenantId ?? 'unknown'}</div>
            <div>User: {user?.emailAddress ?? 'unknown'}</div>
          </div>

          <div className="flex flex-wrap gap-3">
            <Button onClick={() => { void createCheckoutSession(); }} disabled={loading}>
              {loading ? 'Creating...' : 'Create checkout session'}
            </Button>

            {session?.url && (
              <Button variant="outline" asChild>
                <a href={session.url} target="_blank" rel="noreferrer">Open checkout</a>
              </Button>
            )}
          </div>

          {error && <p className="text-sm text-destructive">{error}</p>}

          {session && (
            <div className="rounded-md border bg-muted/40 p-4 text-sm space-y-2">
              <div><span className="font-semibold">Session id:</span> {session.id ?? 'n/a'}</div>
              <div className="break-all"><span className="font-semibold">Checkout url:</span> {session.url ?? 'n/a'}</div>
              <pre className="overflow-auto rounded bg-background p-3 text-xs">{JSON.stringify(session, null, 2)}</pre>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}