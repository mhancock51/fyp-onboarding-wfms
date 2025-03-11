import Api from '@/api';
import WorkflowTemplateBuilder from './WorkflowTemplateBuilder';
import { SetStateAction, useEffect, useState } from 'react';
import WorkflowTask from '@/models/WorkflowTask';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { Spinner } from '@/components/ui/spinner';
import AccountDirectory from '@/models/AccountDirectory';
import AccountDirectoryLookup from '@/components/AccountDirectoryLookup';

export default function CreateWorkflowPage() {
  const [taskTemplates, setTaskTemplates] = useState<TaskTemplate[]>([]);  
  const [loading, setLoading] = useState<boolean>(false);
  const [accountId, setAccountId] = useState<string>("");

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
      <WorkflowTemplateBuilder taskTemplates={taskTemplates}/>
    }
    <div className='flex flex-row gap-2'>
      <AccountDirectoryLookup setAccountId={setAccountId} 
        additionalAccounts={[
          {
            displayName: "Onboarder's account",
            id: 'onboarder_account_id',
            departmentId: '',
            departmentName: ''
          }
        ]}
      />
      <span>{accountId}</span>
    </div>
    </>
  )
}
