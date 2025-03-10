import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Label } from '@/components/ui/label';
import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';

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
    uploadedTimestamp: ''
  }
  const [fileUploadState, setFileUploadState] = useState<FileUploadTaskInstance>(DEFAULT_STATE);
  const [file, setFile] = useState<File | null>(null);

  function handleFileInputChange(event: React.ChangeEvent<HTMLInputElement>) {
    if (event.target.files && event.target.files.length > 0) {
      setFile(event.target.files[0]);
    }
  }

  async function handleFormSubmission() {
    if (file === null) return;
    await Api.updateUploadDocTaskState(props.taskInstanceId, file)
    .then((response) => {
      toast("Uploaded document");
    })
    .catch((error) => {
      toast.error("Failed to upload document");
    })
  }

  useEffect(() => {
    setFileUploadState(props.fileUploadInstance);
  }, [props.fileUploadInstance]);

  return (
    <div className='flex flex-col gap-2 p-2'>
      <div className='flex flex-col gap-2 item-center justify-center mx-auto'>
        <Label>Supported document types: {props.fileUploadTemplate.supportedDocumentType}</Label>
      </div>
      {
        fileUploadState.uploadedTimestamp === "" && props.taskStatus === "open" &&
        <form className='mx-auto flex flex-col gap-2 w-100' onSubmit={async(event: any) => {event.preventDefault(); await handleFormSubmission()}}>
          <input type='file' className='bg-gray-100 p-2 rounded-full cursor-pointer' required accept={props.fileUploadTemplate.supportedDocumentType} onChange={handleFileInputChange}/>
          <Button type='submit'>Upload Document</Button>
        </form>
      }
      {
        (fileUploadState.uploadedTimestamp !== "" || props.taskStatus !== "open") &&
        <span>File uploaded at {fileUploadState.uploadedTimestamp}</span>
      }
    </div>
  )
}
