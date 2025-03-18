import { Badge } from '@/components/ui/badge'
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import TaskInstance from '@/models/tasks/TaskInstance'
import React, { useEffect, useState } from 'react'
import TaskDrawer from './TaskDrawer/TaskDrawer'
import TaskTypeBadge from './TaskTypeBadge'
import Api from '@/api'
import { toast } from 'sonner'
import { Spinner } from '@/components/ui/spinner'
import NoResults from '@/components/NoResults'
import { useDispatch } from 'react-redux'
import { SET_TASK_TYPES } from '@/features/appSlice'
import TaskType from '@/models/tasks/taskType'
import TaskStatusBadge from './TaskStatusBadge'

export default function MyTasksPage() {
  const dispatcher = useDispatch(); 

  const [tasks, setTasks] = useState<TaskInstance[]>([]);
  const [currentTask, setCurrentTask] = useState<TaskInstance | null>(null);
  const [open, setOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  async function fetchTaskInstances() {
    setLoading(true);
    Api.fetchAssignedTaskInstance()
    .then((response) => {
      console.log(response);
      setLoading(false);
      setTasks(response.data.data as TaskInstance[]);
    })
    .catch((error) => {
      setLoading(false);
      toast.error("Failed to load assigned taks");      
    })
  }

  function dueInColor(dueIn: number) {
    if (dueIn < 3) {
      return "bg-red-600";
    }
    else if (dueIn < 7) {
      return "bg-orange-500";
    }
    else {
      return "bg-green-600";
    }
  }  

  useEffect(() => {
    void fetchTaskInstances();
  }, []);

  return (
    <div className='m-4 flex flex-col gap-4'>
      <div className='rounded-3xl bg-sidebar p-8' >
        <h1 className='text-xl text-foreground font-bold m-2'>Your Tasks</h1>
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
              <TableCell style={{textAlign: "center"}}>Due  </TableCell>
              <TableCell style={{textAlign: "center"}}>Workflow</TableCell>
              <TableCell style={{textAlign: "center"}}>Description</TableCell>
            </TableHeader> 
            <TableBody>
              {
                tasks.map((task, index) => (
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
                      <Badge className={`mx-2 py-2 px-4 rounded-full ${dueInColor(-1)}`}>
                        -1 days
                      </Badge>
                    </TableCell>
                    <TableCell width={"100px"}>
                      <Badge className='mx-2 py-2 px-4 rounded-full'>
                        {task.workflowInstanceTemplateName === "" ? "N/A" : task.workflowInstanceTemplateName}
                      </Badge>
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
    </div>
  )
}
