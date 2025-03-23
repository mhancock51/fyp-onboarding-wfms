import React, { useEffect, useState } from 'react'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '../ui/table'
import Api from '@/api'
import { AxiosResponse } from 'axios'
import HTTPresponse from '@/models/HTTPresponse'
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO'
import { toast } from 'sonner'
import { useSelector } from 'react-redux'
import { RootState } from '@/store'
import { Badge } from '../ui/badge'
import { Spinner } from '../ui/spinner'
import { Label } from '../ui/label'
import NoResults from '../NoResults'
import TableActionsDropdown, { DropdownAction } from '../TableActionsDropdown'
import { Button } from '../ui/button'
import { ArrowDownUp } from 'lucide-react'

interface Props {
  actions: DropdownAction[];
  setSelectedWorkflow: React.Dispatch<React.SetStateAction<WorkflowInstanceDTO | null>>;
}

export default function WorkflowInstancesTable(props: Props) {
  const [workflowInstances, setWorkflowInstances] = useState<WorkflowInstanceDTO[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [loaded, setLoaded] = useState<boolean>(false);

  // sort by properties
  const [daysSinceOrder, setDaysSinceOrder] = useState<string>("");
  const [tasksCompletedOrder, setTasksCompletedOrder] = useState<string>("");

  const user = useSelector((state: RootState) => state.app.user);
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);

  async function fetchWorkflowInstances() {
    setLoading(true);
    await Api.fetchWorkflowInstances()
    .then((response: AxiosResponse<HTTPresponse<WorkflowInstanceDTO[], string>>) => {
      setWorkflowInstances(response.data.data as WorkflowInstanceDTO[]);
    })
    .catch((error) => {
      toast.error("Failed to load workflow instances");
    })
    .finally(() => {
      setLoading(false);
      setLoaded(true);
    })
  }

  function daysSince(date: string | Date) {
    const pastDate = new Date(date);
    const today = new Date();  
    // calculate difference in milliseconds and convert to days
    const diffInMs = today.getTime() - pastDate.getTime();
    return Math.floor(diffInMs / (1000 * 60 * 60 * 24));
  }

  function getRoleFromUserId(workflowInstance: WorkflowInstanceDTO) {
    switch(user?.id) {
      case workflowInstance:
        return "Onboarder";
      case workflowInstance.supervisorAccountId:
        return "Supervisor";
      default:
        return "other";
    }
  }

  function getStatusDisplayName(workflowInstance: WorkflowInstanceDTO) {
    if (workflowInstance.workflowTemplate.isOnboardingWF) {
      if (workflowInstance.status === "MAINFLOW") return "ONBOARDING";
      if (workflowInstance.status === "PREFLOW") return "PREBOARDING";
      return workflowInstance.status;
    }
    else {
      return workflowInstance.status;
    }
  }

  function getStatusColor(status: string) {
    switch(status) {
      case "MAINFLOW":
        return "bg-blue-500";
      case "PREFLOW":
        return "bg-orange-600";
      case "COMPLETE":
        return "bg-green-500";
      default:
        return "bg-purple-900";
    }
  }

  function sortByDaysOpenAsc() {
    setWorkflowInstances([...workflowInstances.sort((a: WorkflowInstanceDTO, b: WorkflowInstanceDTO) => (
      daysSince(a.creationTimestamp) - daysSince(b.creationTimestamp)
    ))]);
  }

  function sortByDaysOpenDesc() {
    setWorkflowInstances([...workflowInstances.sort((a: WorkflowInstanceDTO, b: WorkflowInstanceDTO) => (
      daysSince(b.creationTimestamp) - daysSince(a.creationTimestamp)
    ))]);
  }

  function sortByTasksCompletedAsc() {
    setWorkflowInstances([...workflowInstances.sort((a: WorkflowInstanceDTO, b: WorkflowInstanceDTO) => (
      (a.completedTasks / a.workflowTemplate.numberOfTasks) - (b.completedTasks / b.workflowTemplate.numberOfTasks)
    ))]);
  }

  function sortByTasksCompletedDesc() {
    setWorkflowInstances([...workflowInstances.sort((a: WorkflowInstanceDTO, b: WorkflowInstanceDTO) => (
      (b.completedTasks / b.workflowTemplate.numberOfTasks) - (a.completedTasks / a.workflowTemplate.numberOfTasks)
    ))]);
  }

  useEffect(() => {
    if (daysSinceOrder === "asc") {
      sortByDaysOpenAsc();
    }
    else {
      sortByDaysOpenDesc();
    }
  }, [daysSinceOrder]);

  useEffect(() => {
    if (tasksCompletedOrder === "asc") {
      sortByTasksCompletedAsc();
    }
    else {
      sortByTasksCompletedDesc();
    }
  }, [tasksCompletedOrder]);

  useEffect(() => {
    void fetchWorkflowInstances();
  }, []);

  return (
    <>
    {
      loading && !loaded &&
      <div className='flex flex-row gap-2 items-center justify-center py-2'>
        <Spinner/>
        <Label>Loading workflows...</Label>
      </div>
    }
    {
      !loading && loaded && workflowInstances.length === 0 &&
      <NoResults text={'No workflows have been assigned to you'}/>
    }
    {
      !loading && loaded && workflowInstances.length > 0 &&
      <Table className='table-auto w-full'>
        <TableHeader className='justify-start'>
          <TableCell width={400} className='text-center'>Workflow Name</TableCell>
          <TableCell width={50} className='text-center cursor-pointer'>
            <div className='flex flex-row gap-1 justify-center items-center'>
              Status
              <ArrowDownUp/>
            </div>
          </TableCell>
          <TableCell width={50} className='text-center cursor-pointer'>
            <div className='flex flex-row gap-1 justify-center items-center'
              onClick={() => {tasksCompletedOrder === "asc" ? setTasksCompletedOrder("desc") : setTasksCompletedOrder("asc")}} 
            >
              Tasks Completed
              <ArrowDownUp/>
            </div>
          </TableCell>
          <TableCell width={50} className='text-center cursor-pointer'>
            <div className='flex flex-row gap-1 justify-center items-center' 
              onClick={() => {daysSinceOrder === "asc" ? setDaysSinceOrder("desc") : setDaysSinceOrder("asc");}}
            >
              Days Open 
              <ArrowDownUp/>
            </div>
          </TableCell>
          <TableCell width={50} className='text-center'>Your Role</TableCell>
          <TableCell width={50} className='text-center'>Supervisor</TableCell>
          <TableCell width={50} className='text-center'>Onboarder</TableCell>
          <TableCell width={1000}>Overdue Tasks</TableCell>  
          <TableCell width={50}></TableCell>        
        </TableHeader>
        <TableBody>
          {
            workflowInstances.map((instance, index) => (
              <TableRow key={index} className='cursor-pointer hover:brightness-90 hover:rounded-full' onClick={() => {props.setSelectedWorkflow(instance);}}>
                <TableCell>{instance.workflowTemplate.name}</TableCell>
                <TableCell width={50}>
                  <Badge className={`bg-primary py-2 px-4 w-full rounded-full text-[12px] text-primary-foreground flex flex-row gap-2 items-center justify-center ${getStatusColor(instance.status)}`}>
                    {getStatusDisplayName(instance)}
                  </Badge>
                </TableCell>
                <TableCell>
                  <Badge className='bg-primary py-2 px-4 rounded-full text-[12px] text-primary-foreground bg-blue-500 flex flex-row gap-2 items-center justify-center'>
                    {instance.completedTasks} out of {instance.workflowTemplate.numberOfTasks} Tasks
                  </Badge>
                </TableCell>
                <TableCell>
                  <Badge className='bg-primary py-2 px-8 rounded-full text-[12px] text-primary-foreground bg-blue-500 flex flex-row gap-2 items-center justify-center'>
                    {daysSince(instance.creationTimestamp)} Days
                  </Badge>
                </TableCell>
                <TableCell>
                  <Badge className='bg-primary py-2 px-4 rounded-full text-[12px] text-primary-foreground flex flex-row gap-2 items-center justify-center w-full'>
                    {getRoleFromUserId(instance)}
                  </Badge>
                </TableCell>
                <TableCell>
                  <Badge className='bg-primary py-2 px-4 rounded-full text-[12px] text-primary-foreground flex flex-row gap-2 items-center justify-center w-full'>
                    {accounts.find(a => a.id === instance.supervisorAccountId)?.displayName ?? "ERROR"}
                  </Badge>
                </TableCell>
                <TableCell>
                  {
                    instance.onboarderAccountId !== null &&
                    <Badge className='bg-primary py-2 px-4 rounded-full text-[12px] text-primary-foreground flex flex-row gap-2 items-center justify-center w-full'>
                      {accounts.find(a => a.id === instance.onboarderAccountId)?.displayName ?? "N/A"}
                    </Badge>              
                  }
                  {
                    instance.onboarderAccountId === null &&
                    <span className='py-2 px-4 rounded-full text-[12px] flex flex-row gap-2 items-center justify-center'>
                      N/A
                    </span>                    
                  }
                </TableCell>                                
                <TableCell>
                  !!!
                </TableCell>
                <TableCell>
                  <TableActionsDropdown actions={props.actions}/>
                </TableCell>
              </TableRow>
            ))
          }
        </TableBody>
      </Table>
    }
    </>
  )
}
