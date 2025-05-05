import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import React, { useEffect, useState } from 'react'
import DocumentLinkBadge from '../DocumentLinkBadge';
import UploadDocumentForm from '@/components/UploadDocumentForm';
import { ChecklistTaskInstance } from '@/models/tasks/ChecklistTaskInstance';
import ReadDocumentTaskInstance from '@/models/tasks/ReadDocumentTaskInstance';
import ProjectTaskInstance from '@/models/tasks/ProjectTaskInstance';
import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance';

interface Props {
  taskInstanceId: string;
  fileUploadInstance: FileUploadTaskInstance
  fileUploadTemplate: FileUploadTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
  updateTaskInstance: (updatedData: ChecklistTaskInstance | FileUploadTaskInstance | ReadDocumentTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null) => void;  
}

export default function UploadDocumentTask(props: Props) {
  useEffect(() => {    
    if (props.fileUploadInstance.documentId !== "") {
      props.setCanCompleteTask(true);
    }
  }, [props.fileUploadInstance]);

  return (
    <div className='flex flex-col gap-2 p-2'>      
      {
        props.fileUploadInstance.documentId === "" &&
        <UploadDocumentForm allowedFileExtensions={props.fileUploadTemplate.supportedDocumentType} 
          documentName={props.fileUploadTemplate.documentName} 
          accessAccountIds={props.fileUploadTemplate.accessAccountIds}
          taskInstanceId={props.taskInstanceId}
          onSuccessfullUpload={() => {void props.fetchTaskInstances()}}
        />
      }
      {
        props.fileUploadInstance.documentId !== "" &&
        <div className='flex flex-row w-full justify-center'>
          Document uploaded for this task
        </div>        
      }
      {
        props.fileUploadInstance.documentId !== "" &&
        <div className='flex flex-row w-full gap-2 items-center'>
          <span>Uploaded file:</span>
          <DocumentLinkBadge documentId={props.fileUploadInstance.documentId}/>          
        </div>
      }   
    </div>
  )
}