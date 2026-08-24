import TaskType from '@/models/tasks/TaskType';
import { RootState } from '@/store';
import React, { useEffect, useState } from 'react'
import { useSelector } from 'react-redux';
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from './ui/select';

interface Props {
  setTaskType: React.Dispatch<React.SetStateAction<TaskType | null>>;
  value?: string;
}

export default function TaskTypeLookup(props: Props) {
  const [loading, setLoading] = useState<boolean>(false);

  const taskTypes = useSelector((state: RootState) => state.app.taskTypes);

  async function fetchTaskTypes() {
    
  }
  useEffect(() => {
    if (taskTypes.length === 0) {
      void fetchTaskTypes();
    }
    }, []);

  return (
    <div className='flex flex-row gap-2'>
      <Select required value={props.value ?? ""} onValueChange={(value: string) => {props.setTaskType(taskTypes.find(i => i.id === value) ?? null);}}>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select a task type" />
        </SelectTrigger>
        <SelectContent>
          {
            !loading &&
            <SelectGroup>
              <SelectLabel>Task Types</SelectLabel>
              {
                taskTypes.map((taskType, index) => (
                  <SelectItem key={index} value={taskType.id}>{taskType.taskName}</SelectItem>
                ))
              }          
            </SelectGroup>
          }
          {
            loading &&
            <SelectGroup>
              <SelectLabel>Loading task types</SelectLabel>
            </SelectGroup>
          }
        </SelectContent>
      </Select> 
    </div>
  )
}

