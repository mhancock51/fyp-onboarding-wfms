import Api from '@/api'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Label } from '@/components/ui/label'
import { ChecklistTaskInstance } from '@/models/ChecklistTaskInstance'
import { ChecklistTaskTemplate } from '@/models/ChecklistTaskTemplate'
import { CheckedState } from '@radix-ui/react-checkbox'
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner'

interface Props {
  taskInstanceId: string;
  checklistInstance: ChecklistTaskInstance;
  checklistTemplate: ChecklistTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
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

  async function updateChecklistStatus(checklistState: ChecklistTaskInstance) {
    await Api.updateChecklistTaskState(props.taskInstanceId, checklistState.itemCompletionStatuses)
    .then((response) => {
      toast("Successfully updated checklist task's state");
      if (areAllTasksComplete(checklistState.itemCompletionStatuses)) {
        props.setCanCompleteTask(true);
      }
      else {
        props.setCanCompleteTask(false);
      }
      void props.fetchTaskInstances();
    })
    .catch((error) => {
      toast("Failed to update checklist task's state");
    })
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
    if (checklistState.itemCompletionStatuses == props.checklistInstance.itemCompletionStatuses) return;
    void updateChecklistStatus(checklistState);
  }, [checklistState.itemCompletionStatuses]);

  return (
    <div className='flex flex-col gap-2 p-2'>
      {
        props.checklistTemplate.items.map((item, index) => (
          <div key={index} className='flex flex-row gap-2 p-4 rounded-full border-1 border-black items-center cursor-pointer' 
            onClick={() => { updateChecklistItem(index, !checklistState?.itemCompletionStatuses[index])}}
          >
            <Checkbox className='cursor-pointer' checked={checklistState?.itemCompletionStatuses[index]} onCheckedChange={(checked: CheckedState) => { updateChecklistItem(index, checked as boolean)}}/>
            <Label className='font-normal cursor-pointer'>{item}</Label>                    
          </div>
        ))
      }          
    </div>
  )
}
