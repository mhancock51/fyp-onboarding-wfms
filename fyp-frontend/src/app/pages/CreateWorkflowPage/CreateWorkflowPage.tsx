import Api from '@/api';
import WorkflowTemplateBuilder from './WorkflowTemplateBuilder';
import { SetStateAction, useEffect, useState } from 'react';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { Spinner } from '@/components/ui/spinner';
import { Button } from '@/components/ui/button';

export default function CreateWorkflowPage() {
  const [taskTemplates, setTaskTemplates] = useState<TaskTemplate[]>([]);  
  const [loading, setLoading] = useState<boolean>(false);  
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
    <Button onClick={() => {setIsOnboardingWf(!isOnboardingWf);}}>Toggle Onboarding Workflow</Button>
    </>
  )
}
