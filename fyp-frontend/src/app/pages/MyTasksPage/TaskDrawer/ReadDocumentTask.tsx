import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { ChecklistTaskInstance } from '@/models/tasks/ChecklistTaskInstance';
import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance';
import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance';
import ProjectTaskInstance from '@/models/tasks/ProjectTaskInstance';
import ReadDocumentTaskInstance from '@/models/tasks/ReadDocumentTaskInstance';
import { ReadDocumentTaskTemplate } from '@/models/tasks/ReadDocumentTaskTemplate';
import { CheckedState } from '@radix-ui/react-checkbox';
import { Link } from 'lucide-react';
import React, { useEffect, useState } from 'react'

interface Props {
  taskInstanceId: string;
  readDocumentInstance: ReadDocumentTaskInstance;
  readDocumentTemplate: ReadDocumentTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
  updateTaskInstance: (updatedData: ChecklistTaskInstance | FileUploadTaskInstance | ReadDocumentTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null) => void;  
}

export default function ReadDocumentTask(props: Props) {
  const DEFAULT_STATE: ReadDocumentTaskInstance = {
    id: '',
    taskInstanceId: '',
    checkboxChecked: false,
    linkClicked: false
  }
  const [readDocState, setReadDocState] = useState<ReadDocumentTaskInstance>(DEFAULT_STATE);  
  
  function updateCheckboxState(value: boolean) {
    setReadDocState((prevState) => ({
      ...prevState, 
      checkboxChecked: value
    }));
  }
  
  function updateLinkClickedStateOnClick() {
    setReadDocState((prevState) => ({
      ...prevState, 
      linkClicked: true
    }));
  }
  
  function redirectToDocumentLink() {
    window.open(props.readDocumentTemplate.documentUrl, '_blank');
    updateLinkClickedStateOnClick();
  }

  useEffect(() => {
    setReadDocState(props.readDocumentInstance);
  }, [props.readDocumentInstance]);

  useEffect(() => {
    if (readDocState.checkboxChecked && readDocState.linkClicked) {      
      props.updateTaskInstance(readDocState);
      props.setCanCompleteTask(true);
    }
  }, [readDocState.checkboxChecked]);
  
  return (
    <div className='flex flex-col gap-2 p-2'>
      <Button disabled={props.taskStatus !== "open"} onClick={redirectToDocumentLink}>{props.readDocumentTemplate.documentName} <Link/></Button>
      <div className='flex flex-row gap-2 mx-auto'>
        <Checkbox disabled={!readDocState.linkClicked || props.taskStatus !== "open"} checked={readDocState.checkboxChecked} onCheckedChange={(checked: CheckedState) => { updateCheckboxState(checked as boolean);}}/>
        <Label>{props.readDocumentTemplate.checkBoxLabel}</Label>
      </div>  
    </div>  
  )
}
