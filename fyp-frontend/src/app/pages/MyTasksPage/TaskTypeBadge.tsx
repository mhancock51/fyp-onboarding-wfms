import { Badge } from '@/components/ui/badge';
import { TASK_TYPE_IDS } from '@/constants';
import TaskType from '@/models/tasks/TaskType';
import { RootState } from '@/store';
import clsx from 'clsx';
import { ListTodo, FileUp, StickyNote, FileText, Rocket} from 'lucide-react';
import React from 'react'
import { useSelector } from 'react-redux';

interface Props {
  taskTypeId: string;
  className?: string;
}

export default function TaskTypeBadge(props: Props) {
  const taskTypes = useSelector((state: RootState) => state.app.taskTypes);

  function taskTypeIcon(taskType?: string) {
    const ICON_SIZE = 22;
    switch((taskType ?? "").toLowerCase()) {
      case TASK_TYPE_IDS.CHECKLIST:
        return <ListTodo size={ICON_SIZE}/>
      case TASK_TYPE_IDS.UPLOAD_DOCUMENT:
        return <FileUp size={ICON_SIZE}/>
      case TASK_TYPE_IDS.READ_DOCUMENT:
        return <FileText size={ICON_SIZE}/>
      case TASK_TYPE_IDS.PROJECT_TASK:
        return <Rocket size={ICON_SIZE}/>
      default:
        return <StickyNote size={ICON_SIZE}/>
    }    
  }

  return (
    <Badge className={clsx('p-2 w-full rounded-full flex flex-row justify-center gap-2 cursor-pointer min-w-[125px]', props.className)}>
      {taskTypeIcon(props.taskTypeId)}
      {taskTypes.find((taskType: TaskType) => (taskType.id === props.taskTypeId))?.taskName}
    </Badge>
  )
}
