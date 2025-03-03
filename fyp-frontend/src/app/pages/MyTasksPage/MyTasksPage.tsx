import { Badge } from '@/components/ui/badge'
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import TaskInstance from '@/models/TaskInstance'
import { FileUp, ListTodo, StickyNote } from 'lucide-react'
import React, { useEffect, useState } from 'react'
import TaskDrawer from './TaskDrawer'
import TaskTypeBadge from './TaskTypeBadge'
import Api from '@/api'

export default function MyTasksPage() {

  const [tasks, setTasks] = useState<TaskInstance[]>([]);
  const [currentTask, setCurrentTask] = useState<TaskInstance | null>(null);
  const [open, setOpen] = useState<boolean>(false);

  async function fetchTaskInstances() {
    Api.fetchAssignedTaskInstance()
    .then((response) => {
      console.log(response);
      setTasks(response.data.data as TaskInstance[]);
    })
    .catch((error) => {
      console.error(error);
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
                  <TableCell>
                    {task.taskName}
                  </TableCell>
                  <TableCell width={"175px"}>                    
                    <TaskTypeBadge taskType={task.taskType}/>
                  </TableCell>
                  <TableCell width={"100px"}>
                    <Badge className='mx-2 rounded-full text-white bg-blue-500 items-center p-2' style={{minWidth: "90px"}}>
                      {task.status.toUpperCase()}
                    </Badge>
                  </TableCell>
                  <TableCell width={"50px"}>
                    <Badge className={`mx-2 py-2 px-4 rounded-full ${dueInColor(task.dueIn)}`}>
                      {task.dueIn} days
                    </Badge>
                  </TableCell>
                  <TableCell width={"100px"}>
                    <Badge className='mx-2 py-2 px-4 rounded-full'>
                      {task.workflowName}
                    </Badge>
                  </TableCell>
                  <TableCell>
                    {task.description}
                  </TableCell>
                </TableRow>
              ))
            }        
          </TableBody>
        </Table>
      </div>
      <TaskDrawer open={open} setOpen={setOpen} task={currentTask}/>
    </div>
  )
}
