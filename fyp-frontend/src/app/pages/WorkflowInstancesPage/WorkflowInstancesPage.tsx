import DocumentsDialog from '@/app/dialogs/DocumentsDialog';
import WorkflowInstanceAuditDialog from '@/app/dialogs/WorkflowInstanceAuditDialog';
import { DropdownAction } from '@/components/TableActionsDropdown'
import WorkflowInstancesTable from '@/components/Tables/WorkflowInstancesTable'
import { Separator } from '@/components/ui/separator'
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO';
import React, { useState } from 'react'

export default function WorkflowInstancesPage() {
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
        <h1 className='text-xl text-foreground font-bold m-2'>Workflows Assinged To You</h1>
        <Separator/>
        <WorkflowInstancesTable actions={actions} setSelectedWorkflow={setSelectedWorkflow}/>
      </div>
      <DocumentsDialog open={openDocumentsDialog && selectedWorkflow !== null} setOpen={setOpenDocumentsDialog} workflowInstance={selectedWorkflow}/>
      {
        selectedWorkflow !== null &&
        <WorkflowInstanceAuditDialog open={openAuditDialog && selectedWorkflow !== null} setOpen={setOpenAuditDialog} workflowInstance={selectedWorkflow}/>
      }
    </div>
  )
}
