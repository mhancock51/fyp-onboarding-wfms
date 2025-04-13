import { Badge } from '@/components/ui/badge'
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO'
import React, { useEffect, useState } from 'react'
import TaskDrawer from './TaskDrawer/TaskDrawer'
import TaskTypeBadge from './TaskTypeBadge'
import Api from '@/api'
import { toast } from 'sonner'
import { Spinner } from '@/components/ui/spinner'
import NoResults from '@/components/NoResults'
import { useDispatch } from 'react-redux'
import TaskStatusBadge from './TaskStatusBadge'
import ReportIssueDialog from '@/app/dialogs/ReportIssueDialog'
import WorkflowInstanceBadge from '@/components/WorkflowInstanceBadge'

export default function MyTasksPage() {
  const dispatcher = useDispatch(); 

  const [tasks, setTasks] = useState<TaskInstanceDTO[]>([]);
  const [currentTask, setCurrentTask] = useState<TaskInstanceDTO | null>(null);
  const [open, setOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  async function fetchTaskInstances() {
    setLoading(true);
    Api.fetchAssignedTaskInstance()
    .then((response) => {
      console.log(response);
      setLoading(false);
      setTasks(response.data.data as TaskInstanceDTO[]);
    })
    .catch((error) => {
      setLoading(false);
      toast.error("Failed to load assigned taks");      
    })
  }

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

  useEffect(() => {
    void fetchTaskInstances();
  }, []);

  return (
    <div>
      <div>
        <h1 className='text-xl text-foreground font-bold m-2'>Your Tasks ({tasks.filter(t => t.status === "open").length} open)</h1>
        <Separator/>
        {
          loading && tasks.length === 0 &&
          <div className='flex flex-row justify-center p-4 gap-2'>
            <Spinner/>
            <h1>Loading tasks...</h1>
          </div>
        }
        {
          !loading && tasks.length == 0 &&
          <NoResults text='No tasks could be found'/>                    
        }
        {
          !loading && tasks.length > 0 &&
          <Table className='text-base'>
            <TableHeader>
              <TableCell style={{textAlign: "center"}}>Name    </TableCell>
              <TableCell style={{textAlign: "center"}}>Type    </TableCell>
              <TableCell style={{textAlign: "center"}}>Status  </TableCell>
              <TableCell style={{textAlign: "center"}} width={100}>Due  </TableCell>
              <TableCell style={{textAlign: "center"}}>Workflow</TableCell>
              <TableCell style={{textAlign: "center"}}>Description</TableCell>
            </TableHeader> 
            <TableBody>
              {
                tasks.sort((a, b) => (new Date(a.creationTimestamp).getTime() - new Date(b.creationTimestamp).getTime())).map((task, index) => (
                  <TableRow key={index} onClick={() => { setCurrentTask(task); setOpen(true); }} className='cursor-pointer'>
                    <TableCell style={{maxWidth: "150px", overflowX: "hidden", textOverflow: "ellipsis"}}>
                      {task.template.name}
                    </TableCell>
                    <TableCell width={"175px"}>                    
                      <TaskTypeBadge taskTypeId={task.template.taskTypeId}/>
                    </TableCell>
                    <TableCell width={"100px"}>
                      <TaskStatusBadge status={task.status}/>                      
                    </TableCell>
                    <TableCell width={"50px"}>
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
                    <TableCell style={{maxWidth: "200px", overflowX: "hidden", textOverflow: "ellipsis"}}>
                      {task.template.description}
                    </TableCell>
                  </TableRow>
                ))
              }        
            </TableBody>
          </Table>
        }
      </div>
      {
        currentTask !== null &&
        <TaskDrawer open={open} setOpen={setOpen} task={currentTask} fetchTaskInstances={fetchTaskInstances}/>
      }
      {
        currentTask !== null &&
        <ReportIssueDialog taskInstance={currentTask} />
      }
    </div>
  )
}
