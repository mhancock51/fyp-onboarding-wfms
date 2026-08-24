import { Checkbox } from '@/components/ui/checkbox'
import { Label } from '@/components/ui/label'
import { ChecklistTaskInstance } from '@/models/tasks/ChecklistTaskInstance'
import { ChecklistTaskTemplate } from '@/models/tasks/ChecklistTaskTemplate'
import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance'
import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance'
import ProjectTaskInstance from '@/models/tasks/ProjectTaskInstance'
import ReadDocumentTaskInstance from '@/models/tasks/ReadDocumentTaskInstance'
import { CheckedState } from '@radix-ui/react-checkbox'
import React, { useEffect, useState } from 'react'

interface Props {
  taskInstanceId: string;
  checklistInstance: ChecklistTaskInstance;
  checklistTemplate: ChecklistTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
  updateTaskInstance: (updatedData: ChecklistTaskInstance | FileUploadTaskInstance | ReadDocumentTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null) => void;  
}

export default function ChecklistTask(props: Props) {
  const DEFAULT_STATE: ChecklistTaskInstance = {
    id: '',
    taskInstanceId: '',
    itemCompletionStatuses: []
  }
  const [checklistState, setChecklistState] = useState<ChecklistTaskInstance>(DEFAULT_STATE);

  function areAllTasksComplete(taskStatuses: boolean[]) {
    let allCompleted = true;
    taskStatuses.forEach((status) => {
      if (!status) {
        allCompleted = false;
      }
    })
    return allCompleted;
  }

  function updateChecklistItem(index: number, value: boolean) {
    if (props.taskStatus !== "open") return;
    // update checklist item's state through hook
    setChecklistState((prevState) => ({ 
      ...prevState, 
      // find item by index and set its value
      itemCompletionStatuses: prevState.itemCompletionStatuses.map((status, i) => (i === index ? value : status))
    }));
  } 
  

  useEffect(() => {
    setChecklistState(props.checklistInstance);
  }, [props.checklistInstance]);

  useEffect(() => {
    props.setCanCompleteTask(areAllTasksComplete(checklistState.itemCompletionStatuses));
    if (checklistState.id === "") return;
    // exit if state is the same as when loaded
    if (checklistState.itemCompletionStatuses == props.checklistInstance.itemCompletionStatuses) return;
    props.updateTaskInstance(checklistState);    
  }, [checklistState.itemCompletionStatuses]);

  return (
    <div className='flex flex-col gap-2 p-2'>
      {
        props.checklistTemplate.items.map((item, index) => (
          <div key={index} className='min-h-[60px] flex flex-row gap-2 px-4 rounded-[24px] hover:bg-muted/50 border-2 border-accent items-center cursor-pointer' 
            onClick={() => { updateChecklistItem(index, !checklistState?.itemCompletionStatuses[index])}}
          >
            <Checkbox className={'cursor-pointer data-[state=checked]:bg-green-500'} checked={checklistState?.itemCompletionStatuses[index]} onCheckedChange={(checked: CheckedState) => { updateChecklistItem(index, checked as boolean)}}/>
            <Label className='font-normal cursor-pointer'>{item}</Label>                    
          </div>
        ))
      }          
    </div>
  )
}
