import { HoverCard, HoverCardContent, HoverCardTrigger } from './ui/hover-card'
import { Badge } from './ui/badge'
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO'
import { Separator } from './ui/separator';
import { Label } from './ui/label';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import Utils from '@/util';
import clsx from 'clsx';

export default function WorkflowInstanceBadge(props: { workflowInstance: WorkflowInstanceDTO, className?: string;}) {  
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);

  const supervisorAccount = accounts.find(a => a.id === props.workflowInstance?.supervisorAccountId);

  return (
    <HoverCard>
      <HoverCardTrigger>
        <Badge className={clsx('p-2 w-full rounded-full cursor-pointer', props.className)}>
          {props.workflowInstance?.workflowTemplate.name}
        </Badge>
      </HoverCardTrigger>
      {
        props.workflowInstance !== null &&
        <HoverCardContent className='p-2 w-auto'>
          <div className='flex flex-col gap-1 w-auto'>          
            <div className='flex flex-row gap-2 items-center'>
              <h1 className='text-start text-base'>{props.workflowInstance?.workflowTemplate.name}</h1>              
            </div>                    
            <Separator/>
            <div className="grid grid-cols-4 gap-4">
              <Label className='text-sm'>Status</Label>
              <Label className='col-span-3 font-normal text-sm'>{Utils.getWorkflowStatusDisplayName(props.workflowInstance)}</Label>
            </div>
            <div className="grid grid-cols-4 gap-4">
              <Label className='text-sm'>Completed Tasks</Label>
              <Label className='col-span-3 font-normal text-sm'>{props.workflowInstance.completedTasks} of {props.workflowInstance.workflowTemplate.numberOfTasks}</Label>
            </div>
            {              
              props.workflowInstance.onboardingEmployeeDetails !== null &&
              <>
                <div className="grid grid-cols-4 gap-4">
                  <Label className='text-sm'>Onboarder</Label>
                  <Label className='col-span-3 font-normal text-sm'>{props.workflowInstance.onboardingEmployeeDetails?.displayName} ({props.workflowInstance.onboardingEmployeeDetails.emailAddress})</Label>
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
