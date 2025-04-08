import React, { useEffect, useState } from 'react'
import { HoverCard, HoverCardContent, HoverCardTrigger } from './ui/hover-card'
import { Badge } from './ui/badge'
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO'
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import { Spinner } from './ui/spinner';
import { Separator } from './ui/separator';
import { Label } from './ui/label';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import Utils from '@/util';

export default function WorkflowInstanceBadge(props: { workflowInstanceId: string}) {  
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);
  
  const [workflowInstance, setWorkflowInstance] = useState<WorkflowInstanceDTO | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [errored, setErrored] = useState<boolean>(false);


  async function fetchWorkflowInstance(workflowInstanceId: string) {
    setLoading(true);
    await Api.workflowInstances.fetchWorkflowInstance(workflowInstanceId)
    .then((response: AxiosResponse<HTTPresponse<WorkflowInstanceDTO, string>>) => {
      setWorkflowInstance(response.data.data as WorkflowInstanceDTO);
    })
    .catch((error) => {
      setErrored(true);
    })
    .finally(() => {
      setLoading(false);
    })
  }

  useEffect(() => {
    void fetchWorkflowInstance(props.workflowInstanceId);
  }, [props.workflowInstanceId]);

  const supervisorAccount = accounts.find(a => a.id === workflowInstance?.supervisorAccountId);

  return (
    <HoverCard>
      <HoverCardTrigger>
        <Badge className='p-2 w-full rounded-full cursor-pointer'>
          {
            loading && "loading template..."
          }
          {workflowInstance?.workflowTemplate.name}
        </Badge>
      </HoverCardTrigger>
      {
        !loading && workflowInstance !== null &&
        <HoverCardContent className='p-2 w-auto'>
          <div className='flex flex-col gap-1 w-auto'>          
            <div className='flex flex-row gap-2 items-center'>
              <h1 className='text-start text-base'>{workflowInstance?.workflowTemplate.name}</h1>              
            </div>                    
            <Separator/>
            <div className="grid grid-cols-4 gap-4">
              <Label className='text-sm'>Status</Label>
              <Label className='col-span-3 font-normal text-sm'>{Utils.getWorkflowStatusDisplayName(workflowInstance)}</Label>
            </div>
            <div className="grid grid-cols-4 gap-4">
              <Label className='text-sm'>Completed Tasks</Label>
              <Label className='col-span-3 font-normal text-sm'>{workflowInstance.completedTasks} of {workflowInstance.workflowTemplate.numberOfTasks}</Label>
            </div>
            {              
              workflowInstance.onboardingEmployeeDetails !== null &&
              <>
                <div className="grid grid-cols-4 gap-4">
                  <Label className='text-sm'>Onboarder</Label>
                  <Label className='col-span-3 font-normal text-sm'>{workflowInstance.onboardingEmployeeDetails?.displayName} ({workflowInstance.onboardingEmployeeDetails.emailAddress})</Label>
                </div>
                <div className="grid grid-cols-4 gap-4">
                  <Label className='text-sm'>Supervisor</Label>
                  <Label className='col-span-3 font-normal text-sm'>{supervisorAccount?.displayName ?? "ERROR"} ({supervisorAccount?.emailAddress ?? "ERROR"})</Label>
                </div>
              </>
            }            
          </div>
        </HoverCardContent>
      }
    </HoverCard>
  )
}
