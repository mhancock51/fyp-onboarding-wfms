import Api from '@/api'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Label } from '@/components/ui/label'
import { ChecklistTaskInstance } from '@/models/ChecklistTaskInstance'
import { ChecklistTaskTemplate } from '@/models/ChecklistTaskTemplate'
import { CheckedState } from '@radix-ui/react-checkbox'
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner'

export default function ChecklistTask(props: {taskInstanceId: string, checklistInstance: ChecklistTaskInstance, checklistTemplate: ChecklistTaskTemplate}) {
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
    })
    .catch((error) => {
      toast("Failed to update checklist task's state");
    })
  }

  function updateChecklistItem(index: number, value: boolean) {
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
    if (checklistState.id === "") return;
    if (checklistState.itemCompletionStatuses == props.checklistInstance.itemCompletionStatuses) return;
    void updateChecklistStatus(checklistState);
  }, [checklistState.itemCompletionStatuses]);

  return (
    <div className='flex flex-col gap-2 p-2'>
      {
        props.checklistTemplate.items.map((item, index) => (
          <div key={index} className='flex flex-row gap-2 p-4 rounded-full border-1 border-black items-center'>
            <Checkbox checked={checklistState?.itemCompletionStatuses[index]} onCheckedChange={(checked: CheckedState) => {updateChecklistItem(index, checked as boolean)}}/>
            <Label className='font-normal'>{item}</Label>                    
          </div>
        ))
      }
      <Button disabled={!areAllTasksComplete(checklistState.itemCompletionStatuses)}>
        Complete Task
      </Button>              
    </div>
  )
}
