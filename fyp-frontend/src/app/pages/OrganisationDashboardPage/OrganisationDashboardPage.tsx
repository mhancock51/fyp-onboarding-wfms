import DashboardActionCard, { DashboardAction } from '@/components/DashboardActionCard';
import { Button } from '@/components/ui/button';
import { Building, UserPlus, Users } from 'lucide-react';
import { useDispatch } from 'react-redux';

import { SET_OPEN_ACCOUNTS_DIALOG, SET_OPEN_INVITE_DIALOG, SET_OPEN_ORGANISATION_DIALOG } from '@/features/appSlice';

export default function OrganisationDashboardPage() {
  const dispatch = useDispatch();

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

  return (
    <div className="min-h-full bg-gradient-to-br from-background via-background to-muted/30 md:p-6">
      <div className="mx-auto flex max-w-7xl flex-col gap-6">
        <section className="overflow-hidden rounded-3xl border border-border/60 bg-card/80 p-6 shadow-sm backdrop-blur md:p-8">
          <div className="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-3xl space-y-4">
              <div className="space-y-3">
                <h1 className="text-3xl font-bold tracking-tight md:text-4xl">Organisation Management</h1>
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
          {organisationActions.map((action) => (
            <DashboardActionCard key={action.title} {...action} />
          ))}
        </section>
      </div>
    </div>
  );
}