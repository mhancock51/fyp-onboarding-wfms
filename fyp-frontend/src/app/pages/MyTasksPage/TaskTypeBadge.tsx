import { ListTodo, FileUp, StickyNote } from 'lucide-react';
import React from 'react'

export default function TaskTypeBadge(props: {taskType: string}) {
  function taskTypeIcon(taskType: string) {
    const ICON_SIZE = 18;
    switch(taskType.toLowerCase()) {
      case "checklist":
        return <ListTodo size={ICON_SIZE}/>
      case "document upload":
        return <FileUp size={ICON_SIZE}/>
      default:
        return <StickyNote size={ICON_SIZE}/>
    }    
  }

  return (
    <div className='mx-2 bg-primary p-2 rounded-full text-xs text-primary-foreground flex flex-row gap-2 items-center justify-center'>
      {props.taskType}
      {taskTypeIcon(props.taskType)}
    </div>
  )
}
