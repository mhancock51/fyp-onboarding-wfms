import { Separator } from '@/components/ui/separator'
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO'
import React, { useEffect, useState } from 'react'
import TaskDrawer from './TaskDrawer/TaskDrawer'
import Api from '@/api'
import { toast } from 'sonner'
import { useDispatch } from 'react-redux'
import ReportIssueDialog from '@/app/dialogs/ReportIssueDialog'
import TaskInstancesTable from '@/components/Tables/TaskInstancesTable'
import { Accordion, AccordionItem, AccordionTrigger } from '@/components/ui/accordion'
import { AccordionContent } from '@radix-ui/react-accordion'

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

  useEffect(() => {
    void fetchTaskInstances();
  }, []);

  return (
    <div>
      <div className='flex flex-col gap-2'>
        <h1 className='text-xl text-foreground font-bold'>Your Tasks ({tasks.filter(t => t.status === "open").length} open)</h1>
        <div className='flex flex-col gap-1 h-full'>
          <TaskInstancesTable tasks={tasks.filter(t => t.status === "open")} loading={loading} 
            handleTaskClicked={(task: TaskInstanceDTO) => {setCurrentTask(task); setOpen(true);}}
            className='h-[100%] overflow-y-auto'
          />
          {/* Completed tasks accordian */}
          <Accordion type="single" collapsible className="w-full flex-2">
            <AccordionItem value={'item-1'}>
              <AccordionTrigger>
                <div className='flex flex-col gap-2 w-full'>
                  <h2>Completed Tasks ({tasks.filter(t => t.status !== "open").length})</h2>
                  <Separator/>
                </div>
              </AccordionTrigger>
              <AccordionContent>
                <TaskInstancesTable tasks={tasks.filter(t => t.status !== "open")} loading={loading} 
                  handleTaskClicked={(task: TaskInstanceDTO) => {setCurrentTask(task); setOpen(true);}}
                  className='h-[100%] overflow-y-auto'
                  hideDueDate={true}
                />
              </AccordionContent>
            </AccordionItem>
          </Accordion>
        </div>
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
