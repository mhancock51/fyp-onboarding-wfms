import DashboardActionCard, { DashboardAction } from '@/components/DashboardActionCard';
import { Button } from '@/components/ui/button';

import { SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG, SET_OPEN_TASK_TEMPLATES_LIST_DIALOG, SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG } from '@/features/appSlice';
import { Blocks, ChartNoAxesColumn, ClipboardList, LayoutTemplate, ListTodo, Route as WorkflowRouteIcon } from 'lucide-react';
import { useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';

export default function WorkflowDashboardPage() {
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const dashboardActions: DashboardAction[] = [
    {
      title: 'Build workflow template',
      description: 'Design a new workflow from scratch and define its preflow and mainflow task structure.',
      icon: Blocks,
      buttonLabel: 'Open workflow builder',
      onClick: () => { navigate('/workflows/build'); },
      isPrimary: true,
    },
    {
      title: 'Start workflow instance',
      description: 'Launch a new workflow instance for an onboarding or operational process.',
      icon: WorkflowRouteIcon,
      buttonLabel: 'Create instance',
      onClick: () => { dispatch(SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG(true)); }
    },
    {
      title: 'Review workflow templates',
      description: 'Browse existing workflow templates, inspect structure, and update a template when needed.',
      icon: LayoutTemplate,
      buttonLabel: 'Open templates',
      onClick: () => { dispatch(SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG(true)); }
    },
    {
      title: 'Manage task templates',
      description: 'Maintain the task template library used by workflow builders and instance creation.',
      icon: ListTodo,
      buttonLabel: 'Open task templates',
      onClick: () => { dispatch(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(true)); }
    },
    {
      title: 'Assigned workflows',
      description: 'Jump to the workflow list to inspect live instances assigned to you or your team.',
      icon: ClipboardList,
      buttonLabel: 'View workflows',
      onClick: () => { navigate('/workflows'); }
    }
  ];

  return (
    <div className="min-h-full bg-gradient-to-br from-background via-background to-muted/30 md:p-6">
      <div className="mx-auto flex max-w-7xl flex-col gap-6">
        <section className="overflow-hidden rounded-3xl border border-border/60 bg-card/80 p-6 shadow-sm backdrop-blur md:p-8">
          <div className="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-3xl space-y-4">
              <div className="space-y-3">
                <h1 className="text-3xl font-bold tracking-tight md:text-4xl">Workflow Management Dashboard</h1>
                <p className="max-w-2xl text-sm leading-6 text-muted-foreground md:text-base">
                  All the tools you need to manage workflows is centralised here: build templates, manage task templates,
                  launch instances, and move between the workflow tools without hunting through the menu.
                </p>
              </div>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button onClick={() => { navigate('/workflows/build'); }}>
                <Blocks className="h-4 w-4" />
                Build workflow
              </Button>
              <Button variant="secondary" onClick={() => { dispatch(SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG(true)); }}>
                <WorkflowRouteIcon className="h-4 w-4" />
                Start instance
              </Button>
              <Button variant="outline" onClick={() => { dispatch(SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG(true)); }}>
                <ChartNoAxesColumn className="h-4 w-4" />
                Review templates
              </Button>
            </div>
          </div>
        </section>

        <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {dashboardActions.map((action) => (
            <DashboardActionCard key={action.title} {...action} />
          ))}
        </section>
      </div>
    </div>
  )
}
