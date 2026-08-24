import DashboardActionCard, { DashboardAction } from '@/components/DashboardActionCard';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Building, CreditCard, UserPlus, Users } from 'lucide-react';
import { useDispatch, useSelector } from 'react-redux';
import { useEffect, useState } from 'react';

import { SET_OPEN_ACCOUNTS_DIALOG, SET_OPEN_INVITE_DIALOG, SET_OPEN_ORGANISATION_DIALOG, SET_OPEN_MANAGE_SUBSCRIPTION_DIALOG, SET_TENANT_SUBSCRIPTION } from '@/features/appSlice';
import { RootState } from '@/store';
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import TenantSubscription from '@/models/TenantSubscription';
import { usePageTitle } from '@/hooks/usePageTitle';

export default function OrganisationDashboardPage() {
  const dispatch = useDispatch();
  const tenantSubscription = useSelector((state: RootState) => state.app.tenantSubscription);
  const [fetchedSubscription, setFetched] = useState<boolean>(false);

  useEffect(() => {
    Api.tenantSubscription.fetchTenantSubscription()
      .then((response: AxiosResponse<HTTPresponse<TenantSubscription, string>>) => {
        dispatch(SET_TENANT_SUBSCRIPTION(response.data.data));
      })
      .catch((error) => {
        console.error(error);
        // tenant may not have a subscription yet — silently ignore
      })
      .finally(() => {
        setFetched(true);
      });
  }, [dispatch]);

  const subscriptionStatusVariant = (status: string): "default" | "destructive" | "outline" | "secondary" => {
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

  const formatPeriodEnd = (dateStr: string): string => {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  };

  const organisationActions: DashboardAction[] = [
    {
      title: 'Invite user',
      description: 'Invite a new employee into the organisation and assign them to a department.',
      icon: UserPlus,
      buttonLabel: 'Invite employee',
      onClick: () => { dispatch(SET_OPEN_INVITE_DIALOG(true)); },
      isPrimary: false,
    },
    {
      title: 'Manage accounts',
      description: 'Review accounts, search by name or department, and promote users to supervisor.',
      icon: Users,
      buttonLabel: 'Open accounts',
      onClick: () => { dispatch(SET_OPEN_ACCOUNTS_DIALOG(true)); }
    },
    {
      title: 'Manage organisation',
      description: 'Rename the organisation or update its logo so the workspace stays current.',
      icon: Building,
      buttonLabel: 'Open organisation settings',
      onClick: () => { dispatch(SET_OPEN_ORGANISATION_DIALOG(true)); }
    }    
  ];

  const [, setPageTitle] = usePageTitle();
      
    useEffect(() => {
      setPageTitle(`Organisation`);
    }, [setPageTitle]);

  return (
    <div className="min-h-full">
      <div className="mx-auto flex max-w-7xl flex-col gap-6">
        <section className="overflow-hidden rounded-xl border border-border/60 bg-card/80 p-6 backdrop-blur md:p-8">
          <div className="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-3xl space-y-4">
              <div className="space-y-3">
                <p className="max-w-2xl text-sm leading-6 text-muted-foreground md:text-base">
                  All the tool you need to manage your organisation are here: invite new users, manage accounts, and update organisation details from one place.
                </p>
              </div>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button onClick={() => { dispatch(SET_OPEN_INVITE_DIALOG(true)); }}>
                <UserPlus className="h-4 w-4" />
                Invite user
              </Button>
              <Button variant="secondary" onClick={() => { dispatch(SET_OPEN_ACCOUNTS_DIALOG(true)); }}>
                <Users className="h-4 w-4" />
                Manage accounts
              </Button>
              <Button variant="outline" onClick={() => { dispatch(SET_OPEN_ORGANISATION_DIALOG(true)); }}>
                <Building className="h-4 w-4" />
                Organisation settings
              </Button>
            </div>
          </div>
        </section>

        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {/* Subscription card */}
          <DashboardActionCard
            title="Subscription"
            description="View and manage your current plan and billing details."
            icon={CreditCard}
            buttonLabel="Manage subscription"
            onClick={() => { dispatch(SET_OPEN_MANAGE_SUBSCRIPTION_DIALOG(true)); }}
          >
            {tenantSubscription ? (
              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium text-muted-foreground">Plan</span>
                  <div className="flex items-center gap-1">
                    <span className="text-sm font-semibold">{tenantSubscription.subscriptionTier.displayName}</span>
                    <Badge variant={subscriptionStatusVariant(tenantSubscription.stripeSubscriptionStatus)}>
                      {tenantSubscription.stripeSubscriptionStatus}
                    </Badge>
                  </div>
                </div>
                <div className="flex items-center justify-between">
                  <span className="text-sm font-medium text-muted-foreground">Current period ends</span>
                  <span className="text-sm">{formatPeriodEnd(tenantSubscription.subscriptionCurrentPeriodEnd)}</span>
                </div>
              </div>
            ) : (
              <p className="text-sm text-muted-foreground">{fetchedSubscription ? "No active subscription found." : "Fetching subscription..."}</p>
            )}
          </DashboardActionCard>

          {organisationActions.map((action) => (
            <DashboardActionCard key={action.title} {...action} />
          ))}
        </section>
      </div>
    </div>
  );
}