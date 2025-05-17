import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO'
import { Spinner } from '../ui/spinner';
import NoResults from '../NoResults';
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table';
import TaskTypeBadge from '@/app/pages/MyTasksPage/TaskTypeBadge';
import TaskStatusBadge from '@/app/pages/MyTasksPage/TaskStatusBadge';
import { Badge } from '../ui/badge';
import WorkflowInstanceBadge from '../WorkflowInstanceBadge';
import clsx from 'clsx';
import moment from 'moment';
import Utils from '@/util';

interface Props {
  tasks: TaskInstanceDTO[];
  loading: boolean;
  handleTaskClicked: (task: TaskInstanceDTO) => void;
  className?: string;
  hideDueDate?: boolean;
  hideCompletedDate?: boolean;
  noTasksMessage?: string;
  showNewTask?: boolean;
}

export default function TaskInstancesTable(props: Props) {
  function dueInColor(dueIn: number | null) {
    if (dueIn === null) {
      return "bg-accent"
    }
    if (dueIn < 2) {
      return "bg-red-600";
    }
    else if (dueIn < 4) {
      return "bg-orange-500";
    }
    else {
      return "bg-green-600";
    }
  }  

  function dueDisplayValue(dueIn: number | null) {
    if (dueIn === null) return "N/A";
    if (dueIn < 0) return `Overdue (${dueIn * -1} days)`;
    if (dueIn === 0) return "Today";
    if (dueIn === 2) return "Tomorrow";
    return `${dueIn} days`;
  }

  function daysUntil(date: Date | null): number | null {
    if (date === null) return null;
    const targetDate = new Date(date);
    if (isNaN(targetDate.getTime())) {
      throw new Error("Invalid date provided");
    }
    const today = new Date();
    today.setHours(0, 0, 0, 0); // normalize to midnight
    targetDate.setHours(0, 0, 0, 0);  // normalize to midnight
  
    const diffTime = targetDate.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24)); // convert to days
  }

  return (
    <div className={clsx('', props.className)}>
      {
        props.loading &&
        <div className='flex flex-row justify-center p-4 gap-2'>
          <Spinner/>
          Loading tasks...
        </div>
      }
      {
        !props.loading && props.tasks.length == 0 &&
        <NoResults text={props.noTasksMessage ?? 'No tasks could be found'}/>                    
      }
      {
        !props.loading && props.tasks.length > 0 &&
        <Table className='text-base h-[100%]'>
          <TableHeader>
            <TableCell style={{textAlign: "center"}}>Name    </TableCell>
            <TableCell style={{textAlign: "center"}}>Type    </TableCell>
            <TableCell style={{textAlign: "center"}}>Status  </TableCell>
            {
              (props.hideDueDate === undefined || props.hideDueDate === false) &&
              <TableCell style={{textAlign: "center"}} width={100}>Due  </TableCell>
            }
            <TableCell style={{textAlign: "center"}}>Workflow</TableCell>
            <TableCell className='text-center'>Assigned</TableCell>
            <TableCell style={{textAlign: "center"}}>Description</TableCell>
            {
              (props.hideCompletedDate === undefined || props.hideCompletedDate === false) &&
              <TableCell style={{textAlign: "center"}}>Completed</TableCell>
            }
          </TableHeader> 
          <TableBody>
            {
              props.tasks.sort((a, b) => (new Date(a.creationTimestamp).getTime() - new Date(b.creationTimestamp).getTime()))
              .sort((a, b) => (new Date(b.completionTimestamp ?? 0).getTime() - new Date(a.completionTimestamp ?? 0).getTime()))
              .map((task, index) => (
                <TableRow key={index} onClick={() => {props.handleTaskClicked(task)}} className='cursor-pointer'>
                  <TableCell className='overflow-x-hidden text-ellipsis flex flex-row gap-2 items-center'>
                    {
                      props.showNewTask && index === 0 && Utils.minutesSince(new Date(task.creationTimestamp)) < 5 &&
                      <Badge className='bg-blue-500 rounded-full p-1 px-2'>NEW</Badge>
                    }
                    {task.template.name}
                  </TableCell>
                  <TableCell width={"175px"}>                    
                    <TaskTypeBadge taskTypeId={task.template.taskTypeId}/>
                  </TableCell>
                  <TableCell width={"100px"}>
                    <TaskStatusBadge status={task.status}/>                      
                  </TableCell>
                  {
                    (props.hideDueDate === undefined || props.hideDueDate === false) &&
                    <TableCell width={"20px"}>
                      {
                        task.dueDate === null || task.status.toLowerCase() === "complete" &&
                        <div className='text-foreground w-full flex flex-row justify-center text-xs'>
                          N/A
                        </div>
                      }
                      {
                        task.dueDate !== null && task.status.toLowerCase() !== "complete" &&
                        <Badge className={`mx-2 py-2 px-4 rounded-full w-full ${dueInColor(daysUntil(task.dueDate))}`}> 
                          {
                            dueDisplayValue(daysUntil(task.dueDate))
                          }                                      
                        </Badge>
                      }
                    </TableCell>
                  }
                  <TableCell width={"100px"}>
                    {
                      task.workflowInstanceTemplateName === "" &&
                      <div className='text-foreground w-full flex flex-row justify-center'>
                        N/A
                      </div>
                    }
                    {
                      task.workflowInstance !== null &&
                      <WorkflowInstanceBadge workflowInstance={task.workflowInstance}/>
                    }
                  </TableCell>
                  <TableCell>
                    {moment(task.creationTimestamp).fromNow()}                    
                  </TableCell>
                  <TableCell style={{maxWidth: "200px", overflowX: "hidden", textOverflow: "ellipsis"}}>
                    {task.template.description}
                  </TableCell>
                  {
                    (props.hideCompletedDate === undefined || props.hideCompletedDate === false) &&
                    <TableCell>
                      {
                        task.completionTimestamp === null &&
                        <div className='text-foreground w-full flex flex-row justify-center'>
                          N/A
                        </div>
                      }
                      {
                        task.completionTimestamp !== null &&
                        <div className='text-foreground w-full flex flex-row justify-center'>
                          {
                            moment(new Date(task.completionTimestamp)).fromNow()
                          }
                        </div>
                      }
                    </TableCell>
                  }
                </TableRow>
              ))
            }        
          </TableBody>
        </Table>
      }
    </div>
  )
}
