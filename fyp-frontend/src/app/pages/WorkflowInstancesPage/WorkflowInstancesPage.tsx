import DocumentsDialog from '@/app/dialogs/DocumentsDialog';
import { DropdownAction } from '@/components/TableActionsDropdown'
import WorkflowInstancesTable from '@/components/Tables/WorkflowInstancesTable'
import { Separator } from '@/components/ui/separator'
import WorkflowInstanceDTO from '@/models/WorkflowInstanceDTO';
import React, { useState } from 'react'

export default function WorkflowInstancesPage() {
  const actions: DropdownAction[] = [
    {
      label: "View Documents",
      onClick: () => {setOpenDialog(true)}
    }
  ];
  
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  const [selectedWorkflow, setSelectedWorkflow] = useState<WorkflowInstanceDTO | null>(null);

  return (
    <div className='m-4 flex flex-col gap-4'>
      <div className='rounded-3xl bg-accent p-8' >
        <h1 className='text-xl text-foreground font-bold m-2'>Workflows Assinged To You</h1>
        <Separator/>
        <WorkflowInstancesTable actions={actions} setSelectedWorkflow={setSelectedWorkflow}/>
      </div>
      <DocumentsDialog open={openDialog && selectedWorkflow !== null} setOpen={setOpenDialog} workflowInstance={selectedWorkflow}/>
    </div>
  )
}
