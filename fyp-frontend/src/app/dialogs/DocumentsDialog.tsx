import Api from '@/api';
import DocumentsTable from '@/components/Tables/DocumentsTable';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import DocumentDTO from '@/models/DTOs/DocumentDTO';
import HTTPresponse from '@/models/HTTPresponse';
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO';
import { Separator } from '@radix-ui/react-separator';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>
  workflowInstance: WorkflowInstanceDTO | null;
}

export default function DocumentsDialog(props: Props) {
  const [loading, setLoading] = useState<boolean>(false);
  const [loaded, setLoaded] = useState<boolean>(false);
  const [documents, setDocuments] = useState<DocumentDTO[]>([]);

  async function fetchDocuments() {
    if (props.workflowInstance === null) return;
    setLoading(true);
    Api.fetchWorkflowsDocuments(props.workflowInstance?.id)
    .then((response: AxiosResponse<HTTPresponse<DocumentDTO[], string>>) => {      
      setDocuments(response.data.data);
    })
    .catch((error) => {
      toast.error("Failed to load documents");
    })
    .finally(() => {
      setLoading(false);
      setLoaded(true);
    })
  }

  useEffect(() => {
    if (props.workflowInstance !== null) {
      void fetchDocuments();
    }
  }, [props.workflowInstance]);

  function closeAndClear() {
    props.setOpen(false);
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className='min-w-[900px]'>
        <DialogHeader>
          <DialogTitle>Workflow Documents ({props.workflowInstance?.workflowTemplate.name})</DialogTitle>          
          <h1>Displaying documents you have access to</h1>
          <Separator/>
        </DialogHeader> 
        <div className="grid gap-4 py-4">          
          <DocumentsTable documents={documents} loading={loading} loaded={loaded}/>
        </div>
      </DialogContent>
    </Dialog>
  )
}
