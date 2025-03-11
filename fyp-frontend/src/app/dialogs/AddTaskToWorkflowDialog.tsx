import AccountDirectoryLookup from '@/components/AccountDirectoryLookup';
import { MultiSelect } from '@/components/multi-select';
import TaskTemplatesTable from '@/components/Tables/TaskTemplatesTable';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { HoverCard, HoverCardTrigger } from '@/components/ui/hover-card';
import { Label } from '@/components/ui/label';
import AccountDirectory from '@/models/AccountDirectory';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import WorkflowTask from '@/models/WorkflowTask';
import WorkflowTemplateNode from '@/models/WorkflowTemplateNode';
import { HoverCardContent } from '@radix-ui/react-hover-card';
import React, { SetStateAction, useState } from 'react'

interface Props {
  open: boolean;
  setOpen: React.Dispatch<SetStateAction<boolean>>
  onAddTask: (taskTemplate: TaskTemplate, assingee: AccountDirectory, taskDependencies: WorkflowTemplateNode[]) => void;
  isOnboardingWorkflow: boolean;
  existingTaskNodes: WorkflowTemplateNode[];
}

export default function AddTaskToWorkflowDialog(props: Props) {
  const TEMPLATED_ACCOUNTS = [
    {
      displayName: "Onboarder's account",
      id: 'onboarder_account_id',
      departmentId: '',
      departmentName: ''
    },
    {
      displayName: "Workflow Creator's account",
      id: "workflow_creators_account_id",
      departmentId: '',
      departmentName: ''
    }
  ]

  const [step, setStep] = useState<number>(0);
  const [taskTemplate, setTaskTemplate] = useState<TaskTemplate | null>(null);
  const [assignee, setAssignee] = useState<AccountDirectory | null>(null);
  const [taskNodeDependencies, setTaskNodeDependencies] = useState<WorkflowTemplateNode[]>([]);
 
  function closeAndClear() {
    setStep(0);
    setTaskTemplate(null);
    setAssignee(null);
    setTaskNodeDependencies([]);
    props.setOpen(false);
  }

  function addTaskToWorkflow() {
    if (taskTemplate === null) return;
    if (assignee === null) return;
    props.onAddTask(taskTemplate, assignee, taskNodeDependencies);
    closeAndClear();
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className={step === 0 ? "sm:max-w-[750px]" : "sm:max-w-[500px]"}>
        <DialogHeader>
          <DialogTitle>Add Task to Workflow Template</DialogTitle>
        </DialogHeader>
        {
          step === 0 &&
          <div className='flex flex-col gap-2'>
            <TaskTemplatesTable onRowClick={(taskTemplate: TaskTemplate) => {setTaskTemplate(taskTemplate);}} selectedTemplate={taskTemplate}/>
            <Button disabled={taskTemplate === null} onClick={() => {setStep(1)}}>Next</Button>
          </div>
        }
        {
          step === 1 &&
          <form className='flex flex-col gap-2 w-full' onSubmit={(event: any) => { event.preventDefault(); addTaskToWorkflow();}}>
            <div className="grid grid-cols-2 items-center gap-4">
              <Label className="text-right">Task Assignee</Label>
              <AccountDirectoryLookup setAccount={setAssignee} additionalAccounts={props.isOnboardingWorkflow ? TEMPLATED_ACCOUNTS : []}/>                     
            </div>  
            <div className="grid grid-cols-2 gap-4">
              <HoverCard>
                <HoverCardTrigger>
                  <Label className="text-right cursor-pointer">Required?</Label>
                </HoverCardTrigger>
                <HoverCardContent>
                  <Card className='w-75 text-xs p-1.5' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
                    Required tasks will have to be completed for a workflow to be complete
                  </Card>                                
                </HoverCardContent>
              </HoverCard>              
              <Checkbox/>
            </div>  
            <div className="grid grid-cols-2 gap-4">
              <HoverCard>
                <HoverCardTrigger>
                  <Label>Task Dependencies</Label>
                </HoverCardTrigger>
                <HoverCardContent>
                  <Card className='w-75 text-xs p-1.5' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
                    Select the tasks that must be completed before this task can be started
                  </Card>
                </HoverCardContent>
              </HoverCard>
              <MultiSelect 
                options={props.existingTaskNodes.map((node) => ({ label: node.taskTemplate.name, value: node.id}))} 
                onValueChange={(taskIds: string[]) => { setTaskNodeDependencies(props.existingTaskNodes.filter(i => taskIds.includes(i.id))) }}
              />
            </div>
            <DialogFooter>
              <Button type='button' onClick={() => {setStep(0);}}>Back</Button>
              <Button type='submit'>Add to Workflow</Button>
            </DialogFooter>
          </form>
        }

      </DialogContent>
    </Dialog>
  )
}
