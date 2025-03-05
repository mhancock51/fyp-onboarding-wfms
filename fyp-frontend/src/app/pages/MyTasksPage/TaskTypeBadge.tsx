import TaskType from '@/models/tasks/taskType';
import { RootState } from '@/store';
import { ListTodo, FileUp, StickyNote, FileText } from 'lucide-react';
import React from 'react'
import { useSelector } from 'react-redux';

export default function TaskTypeBadge(props: {taskTypeId: string}) {
  const taskTypes = useSelector((state: RootState) => state.app.taskTypes);

  function taskTypeIcon(taskType: string) {
    const ICON_SIZE = 18;
    switch(taskType.toLowerCase()) {
      case "checklist":
        return <ListTodo size={ICON_SIZE}/>
      case "document upload":
        return <FileUp size={ICON_SIZE}/>
      case "read document":
        return <FileText size={ICON_SIZE}/>
      default:
        return <StickyNote size={ICON_SIZE}/>
    }    
  }

  return (
    <div className='mx-2 bg-primary py-2 px-4 rounded-full text-xs text-primary-foreground flex flex-row gap-2 items-center justify-center'>
      {taskTypes.find((taskType: TaskType) => (taskType.id === props.taskTypeId))?.taskName}
      {taskTypeIcon(props.taskTypeId)}
    </div>
  )
}
