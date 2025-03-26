import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';
import DocumentLinkBadge from '../DocumentLinkBadge';
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
    documentId: '',
    uploadedTimestamp: null
  }
  const [fileUploadState, setFileUploadState] = useState<FileUploadTaskInstance>(DEFAULT_STATE);
  const [file, setFile] = useState<File | null>(null);

  function handleFileInputChange(event: React.ChangeEvent<HTMLInputElement>) {
    if (event.target.files && event.target.files.length > 0) {
      setFile(event.target.files[0]);
    }
  }

  async function updateTaskState() {
    await Api.updateTaskState(fileUploadState, "upload-document", props.taskInstanceId)
    .then((response) => {
      if (fileUploadState.documentId !== null) {
        props.setCanCompleteTask(true);

      }
    })
    .catch((error) => {      
      toast.error("Failed to upload document");
    })
  }

  async function handleFormSubmission() {
    if (file === null) return;
    await Api.documents.uploadDocuments({
      taskInstanceId: props.taskInstanceId,
      file: file,
      documentName: props.fileUploadTemplate.documentName,
      accessAccountIds: props.fileUploadTemplate.accessAccountIds
    })
    .then((response: AxiosResponse<HTTPresponse<DocumentDTO, string>>) => {
      var document = (response.data.data as DocumentDTO);
      if (document !== null) {
        const updatedState = {...fileUploadState};
        updatedState.documentId = document.id;
        setFileUploadState(updatedState);     
        props.setCanCompleteTask(true);   
        void props.fetchTaskInstances();
      }
    })
    .catch((error) => {
      toast.error("Failed to upload document");
    });
  }

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
        fileUploadState.documentId !== "" &&
        <div className='flex flex-col gap-2 items-centers'>
          <span>Uploaded file(s)</span>
          <DocumentLinkBadge documentId={fileUploadState.documentId}/>          
        </div>
      }
      {
        props.taskStatus === "open" &&
        <>
          <div className='flex flex-col gap-2 item-center justify-center mx-auto'>
            <Label>Supported document types: {props.fileUploadTemplate.supportedDocumentType}</Label>
          </div>
          {
            fileUploadState.documentId === "" &&
            <form className='mx-auto flex flex-col gap-2 w-100' onSubmit={async(event: any) => {event.preventDefault(); await handleFormSubmission()}}>
              <input type='file' className='bg-gray-100 p-2 rounded-full cursor-pointer' required accept={props.fileUploadTemplate.supportedDocumentType} onChange={handleFileInputChange}/>
              <Button type='submit'>Upload Document</Button>
            </form>
          }
        </>
      }
      {
        props.taskStatus !== "open" &&
        <div className='text-center'>File uploaded at {fileUploadState.uploadedTimestamp?.toLocaleString()}</div>
      }
    </div>
  )
}
