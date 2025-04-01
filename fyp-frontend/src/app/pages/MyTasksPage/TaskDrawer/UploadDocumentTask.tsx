import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';
import DocumentLinkBadge from '../DocumentLinkBadge';
import UploadDocumentForm from '@/components/UploadDocumentForm';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import DocumentDTO from '@/models/DTOs/DocumentDTO';

interface Props {
  taskInstanceId: string;
  fileUploadInstance: FileUploadTaskInstance
  fileUploadTemplate: FileUploadTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
}

export default function UploadDocumentTask(props: Props) {
  const DEFAULT_STATE: FileUploadTaskInstance = {
    id: '',
    taskInstanceId: '',
    documentId: ''
  }
  const [fileUploadState, setFileUploadState] = useState<FileUploadTaskInstance>(DEFAULT_STATE);  

  useEffect(() => {
    setFileUploadState(props.fileUploadInstance);
    if (props.fileUploadInstance.documentId !== "") {
      props.setCanCompleteTask(true);
    }
  }, [props.fileUploadInstance]);

  useEffect(() => {
    if (fileUploadState.uploadedTimestamp === null) return;
    if (fileUploadState === props.fileUploadInstance) return;

    void updateTaskState();
  }, [fileUploadState]);

  return (
    <div className='flex flex-col gap-2 p-2'>      
      {
        fileUploadState.documentId === "" &&
        <UploadDocumentForm allowedFileExtensions={props.fileUploadTemplate.supportedDocumentType} 
          documentName={props.fileUploadTemplate.documentName} 
          accessAccountIds={props.fileUploadTemplate.accessAccountIds}
          taskInstanceId={props.taskInstanceId}
          onSuccessfullUpload={() => {props.setCanCompleteTask(true)}}
        />
      }
      {
        fileUploadState.documentId !== "" &&
        <div className='flex flex-row w-full justify-center'>
          Document uploaded for this task
        </div>        
      }
      {
        fileUploadState.documentId !== "" &&
        <div className='flex flex-row w-full gap-2 items-center'>
          <span>Uploaded file:</span>
          <DocumentLinkBadge documentId={fileUploadState.documentId}/>          
        </div>
      }   
    </div>
  )
}
