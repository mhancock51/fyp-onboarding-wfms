import { Separator } from '@/components/ui/separator'
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO'
import { useEffect, useState } from 'react'
import TaskDrawer from './TaskDrawer/TaskDrawer'
import Api from '@/api'
import { toast } from 'sonner'
import ReportIssueDialog from '@/app/dialogs/ReportIssueDialog'
import TaskInstancesTable from '@/components/Tables/TaskInstancesTable'
import { Accordion, AccordionItem, AccordionTrigger } from '@/components/ui/accordion'
import { AccordionContent } from '@radix-ui/react-accordion'
import { usePageTitle } from '@/hooks/usePageTitle'

export default function MyTasksPage() {
  const [openTasks, setOpenTasks] = useState<TaskInstanceDTO[]>([]);
  const [closedTasks, setClosedTasks] = useState<TaskInstanceDTO[]>([]);
  const [currentTask, setCurrentTask] = useState<TaskInstanceDTO | null>(null);
  const [open, setOpen] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  async function fetchTaskInstances() {
    setLoading(true);
    Api.fetchAssignedTaskInstance()
    .then((response) => {      
      setLoading(false);
      const tasks = response.data.data as TaskInstanceDTO[];
      setOpenTasks(tasks.filter(t => t.status === "open"));
      setClosedTasks(tasks.filter(t => t.status !== "open"));
    })
    .catch(() => {
      setLoading(false);
      toast.error("Failed to load assigned taks");      
    })
  }

  const [, setPageTitle] = usePageTitle();

  useEffect(() => {
    setPageTitle(`Tasks (${openTasks.length} open)`);
  }, [setPageTitle, openTasks]);

  useEffect(() => {
    void fetchTaskInstances();
  }, []);

  return (
    <div>
      <div className='flex flex-col gap-2'>
        <div className='flex flex-col gap-1 h-full'>
          <TaskInstancesTable tasks={openTasks} loading={loading} 
            handleTaskClicked={(task: TaskInstanceDTO) => {setCurrentTask(task); setOpen(true);}}
            className='h-[100%] overflow-y-auto'
            hideCompletedDate={true}
            noTasksMessage='No open tasks assigned to you'
            showNewTask={true}
          />
          {/* Completed tasks accordian */}
          <Accordion type="single" collapsible className="w-full flex-2">
            <AccordionItem value={'item-1'}>
              <AccordionTrigger className='hover:no-underline cursor-pointer'>
                <div className='flex flex-col gap-2 w-full'>
                  <div className='flex flex-row gap-2 justify-left p-[6px] rounded-full items-center hover:bg-muted/50 w-fit'>
                    <h2 className='text-[16px] font-semibold'>Completed Tasks</h2>
                    <div className='bg-primary rounded-full text-background w-8 py-[2px] text-center'>{closedTasks.length}</div>
                  </div>
                  <Separator/>
                </div>
              </AccordionTrigger>
              <AccordionContent>
                <TaskInstancesTable tasks={closedTasks} loading={loading} 
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
