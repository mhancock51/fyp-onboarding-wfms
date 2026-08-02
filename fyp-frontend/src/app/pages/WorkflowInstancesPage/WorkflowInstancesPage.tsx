import DocumentsDialog from '@/app/dialogs/DocumentsDialog';
import WorkflowInstanceAuditDialog from '@/app/dialogs/WorkflowInstanceAuditDialog';
import { DropdownAction } from '@/components/TableActionsDropdown'
import WorkflowInstancesTable from '@/components/Tables/WorkflowInstancesTable'
import { usePageTitle } from '@/hooks/usePageTitle';
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO';
import { useEffect, useState } from 'react'

export default function WorkflowInstancesPage() {
  const [, setPageTitle] = usePageTitle();
  
    useEffect(() => {
      setPageTitle(`Workflows`);
    }, [setPageTitle]);

  const actions: DropdownAction[] = [
    {
      label: "View Documents",
      onClick: () => {setOpenDocumentsDialog(true)}
    },
    {
      label: "View History",
      onClick: () => {setOpenAuditDialog(true)}
    }
  ];
  
  const [openDocumentsDialog, setOpenDocumentsDialog] = useState<boolean>(false);
  const [openAuditDialog, setOpenAuditDialog] = useState<boolean>(false);

  const [selectedWorkflow, setSelectedWorkflow] = useState<WorkflowInstanceDTO | null>(null);

  return (
    <div>
      <div>        
        <WorkflowInstancesTable actions={actions} setSelectedWorkflow={setSelectedWorkflow}/>
      </div>
      {
        selectedWorkflow !== null &&
        <>
          <DocumentsDialog open={openDocumentsDialog} setOpen={setOpenDocumentsDialog} workflowInstance={selectedWorkflow}/>
          <WorkflowInstanceAuditDialog open={openAuditDialog && selectedWorkflow !== null} setOpen={setOpenAuditDialog} workflowInstance={selectedWorkflow}/>
        </>
      }
    </div>
  )
}
