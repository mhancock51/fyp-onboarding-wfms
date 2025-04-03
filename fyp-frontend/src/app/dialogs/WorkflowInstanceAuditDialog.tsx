import { Dialog, DialogContent, DialogTitle } from '@/components/ui/dialog'
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO';
import React from 'react'

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  workflowInstance: WorkflowInstanceDTO;
}

export default function WorkflowInstanceAuditDialog(props: Props) {
  function closeAndClear() {

  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent>
        <DialogTitle>Audit Trail</DialogTitle>
        <div className="grid gap-4 py-4">
          
        </div>
      </DialogContent>
    </Dialog>
  )
}
