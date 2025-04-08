import AccountDirectoryLookup from '@/components/AccountDirectoryLookup';
import TaskTemplatesTable from '@/components/Tables/TaskTemplatesTable';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { HoverCard, HoverCardTrigger } from '@/components/ui/hover-card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { TEMPLATE_ACCOUNTS } from '@/constants';
import AccountDirectory from '@/models/AccountDirectory';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode';
import { HoverCardContent } from '@radix-ui/react-hover-card';
import Select, { MultiValue } from 'react-select';
import { X } from 'lucide-react';
import React, { SetStateAction, useState } from 'react'

interface Props {
  open: boolean;
  setOpen: React.Dispatch<SetStateAction<boolean>>
  onAddTask: (taskTemplate: TaskTemplate, assingee: AccountDirectory, taskDependencies: WorkflowTemplateNode[], daysUntilDue: number | null) => void;
  isOnboardingWorkflow: boolean;
  existingTaskNodes: WorkflowTemplateNode[];
  section: string;
}

export default function AddTaskToWorkflowDialog(props: Props) {  

  const [step, setStep] = useState<number>(0);
  const [taskTemplate, setTaskTemplate] = useState<TaskTemplate | null>(null);
  const [assignee, setAssignee] = useState<AccountDirectory | null>(null);
  const [taskNodeDependencies, setTaskNodeDependencies] = useState<WorkflowTemplateNode[]>([]);

  const [daysUntilDue, setDaysUntilDue] = useState<number | null>(null);
 
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
    props.onAddTask(taskTemplate, assignee, taskNodeDependencies, daysUntilDue);
    closeAndClear();
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent style={{minWidth: step === 0 ? "900px" : "400px"}}>
        <DialogHeader>
          <DialogTitle>Add Task to Workflow Template</DialogTitle>
        </DialogHeader>
        {
          step === 0 &&
          <div className='flex flex-col gap-2 max-h-[70vh] w-auto'>
            <TaskTemplatesTable onRowClick={(taskTemplate: TaskTemplate) => {setTaskTemplate(taskTemplate);}} selectedTemplate={taskTemplate} status='active'/>
            <Button>Create Task Template</Button>
            <Button disabled={taskTemplate === null} onClick={() => {setStep(1)}}>Next</Button>
          </div>
        }
        {
          step === 1 &&
          <form className='flex flex-col gap-2 w-full' onSubmit={(event: any) => { event.preventDefault(); addTaskToWorkflow();}}>
            <div className="grid grid-cols-4 gap-4">
              <Label>Assignee</Label>
              <AccountDirectoryLookup setAccount={setAssignee} account={assignee}
                additionalAccounts={props.isOnboardingWorkflow ? TEMPLATE_ACCOUNTS : []}
              />                     
            </div>  
            {
              props.existingTaskNodes.length > 0 &&
              <div className="grid grid-cols-4 gap-4 w-full">
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
                <Select isMulti className='col-span-3 rounded-lg'
                  value={taskNodeDependencies.map((dependency) => ({
                    label: dependency.taskTemplate?.name ?? "ERROR",
                    value: dependency.id
                  }))}
                  options={props.existingTaskNodes.map((node) => ({
                    label: node.taskTemplate?.name ?? "ERROR",
                    value: node.id
                  }))}
                  onChange={(options: MultiValue<{label: string; value: string}>) => {
                    const selectedIds = options.map(option => option.value);
                    const selectedNodes = props.existingTaskNodes.filter(node => selectedIds.includes(node.id));
                    setTaskNodeDependencies(selectedNodes);
                  }}
                />
              </div>
            }
            <div className="grid grid-cols-4 gap-4">
              <div className='flex flex-col gap-2'>
                <Label>Days Until Due</Label>
                <Label className='font-normal'>(after {props.section} starts)</Label>
              </div>
              <div className='col-span-3 flex flex-row gap-1 items-center'>
                <Input className='flex-8' type='number' min={1} value={daysUntilDue ?? ""} onChange={(event: any) => {setDaysUntilDue(event.target.value);}}/>
                <Label className='font-normal flex-3'>Day(s)</Label>
                <Button className='flex-1' variant={"destructive"} onClick={() => {setDaysUntilDue(null);}}><X/></Button>
              </div>
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
