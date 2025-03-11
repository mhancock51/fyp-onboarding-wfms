import Api from '@/api';
import WorkflowTemplateBuilder from './WorkflowTemplateBuilder';
import { SetStateAction, useEffect, useState } from 'react';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { Spinner } from '@/components/ui/spinner';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Checkbox } from '@/components/ui/checkbox';
import { CheckedState } from '@radix-ui/react-checkbox';

export default function CreateWorkflowPage() {
  const [taskTemplates, setTaskTemplates] = useState<TaskTemplate[]>([]);  
  const [loading, setLoading] = useState<boolean>(false);  
  
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [isOnboardingWf, setIsOnboardingWf] = useState<boolean>(false);  

  async function fetchTaskTemplates() {
    setLoading(true);
    Api.fetchAllTaskTemplates()
    .then((response) => {
      setTaskTemplates(response.data.data);
      setLoading(false);
    })
    .catch((error) => {
      setLoading(false);
    })
  }

  useEffect(() => {    
    void fetchTaskTemplates();
  }, []);

  return (
    <div className='flex flex-col gap-2 w-full items-center'>
      <div className='flex flex-col gap-2 w-1/2'>
        <div className='flex flex-row gap-2 items-center'>
          <div className='flex flex-col gap-1 flex-9'>
            <Label className='flex-3 text-lg'>Workflow Name</Label>
            <Input className='flex-9' type="text" value={name} onChange={(event: any) => {setName(event.target.value)}}/>
          </div>
          <div className='flex flex-col gap-1 flex-3 items-start'>
            <Label>Is Onboarding Workflow?</Label>
            <Checkbox checked={isOnboardingWf} onCheckedChange={(checked: CheckedState) => {setIsOnboardingWf(checked as boolean);}}/>
          </div>
        </div>
        <div className='flex flex-col gap-1'>
          <Label className='flex-3 text-lg'>Description</Label>
          <Textarea className='flex-9' value={description} onChange={(event: any) => {setDescription(event.target.value);}}/>
        </div>        
      </div>
      <>
      {
        loading &&
        <div className='flex flex-row gap-2'>
          <Spinner/>
          <span>Loading...</span>
        </div>
      }
      {
        !loading &&
        <WorkflowTemplateBuilder taskTemplates={taskTemplates} isOnboardingWorkflow={isOnboardingWf}/>
      }         
      </>
      <Button className='mx-2'>Save Workflow Template</Button>
    </div>
  )
}
