import Api from '@/api';
import WorkflowTemplateBuilder from './WorkflowTemplateBuilder';
import { SetStateAction, useEffect, useState } from 'react';
import { Input } from '@/components/ui/input';
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode';
import { toast } from 'sonner';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import { useSearchParams } from 'react-router-dom';
import { WorkflowTemplateNodeDTO } from '@/models/DTOs/WorkflowTemplateNodeDTO';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import { PLACEHOLDER_ONBOARDERS_ACCOUNT, PLACEHOLDER_SUPERVISORS_ACCOUNT } from '@/constants';
import { CreateWorkflowTemplatePayload } from '@/models/payloads/CreateWorkflowTemplatePayload';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Spinner } from '@/components/ui/spinner';

export default function CreateWorkflowPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  
  const [loading, setLoading] = useState<boolean>(false); 
  const [updatingWorkflow, setUpdatingWorkflow] = useState<boolean>(false);

  const [errored, setErrored] = useState<boolean>(false); 
  const [error, setError] = useState<string>("");
  
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [isOnboardingWf, setIsOnboardingWf] = useState<boolean>(false); 
  
  const [preflowTasks, setPreflowTasks] = useState<WorkflowTemplateNode[]>([]);
  const [mainflowTasks, setMainflowTasks] = useState<WorkflowTemplateNode[]>([]);

  const accountsDirectory = useSelector((state: RootState) => state.app.accountsDirectory);
  const taskTemplates = useSelector((state: RootState) => state.app.taskTemplates);


  async function createWorkflowTemplate() {    
    setLoading(true);
    const payload: CreateWorkflowTemplatePayload = {
      id: "",
      name: name,
      description: description,
      isOnboardingWF: isOnboardingWf,
      preflowNodes: preflowTasks.map((task) => (
        { 
          id: "",
          taskTemplateId: task.taskTemplate?.id ?? "",
          assigneeId: task.assignee?.id ?? "",
          dependencyNodeIds: task.taskDependencies.map((dependency) => dependency.taskTemplate?.id ?? "") ?? [],
          daysUntilDue: task.daysUntilDue
        }
      )),
      mainflowNodes: mainflowTasks.map((task) => (
        { 
          id: "",
          taskTemplateId: task.taskTemplate?.id ?? "",
          assigneeId: task.assignee?.id ?? "",
          dependencyNodeIds: task.taskDependencies.map((dependency) => dependency.taskTemplate?.id ?? "") ?? [],
          daysUntilDue: task.daysUntilDue
        }
      )),
    }
    await Api.createWorkflowTemplate(payload)
    .then((response) => {
      toast.success("Successfully created workflow template");
    })
    .catch((error) => {
      toast.error("Failed to create workflow template");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function fetchWorkflowTemplate(workflowTemplateId: string) {
    console.log("fetching workflow template");
    setLoading(true);
    await Api.fetchWorkflowTemplate(workflowTemplateId)
    .then((response) => {      
      var workflowDTO = response.data.data as WorkflowTemplateDTO;
      setName(workflowDTO.name);
      setDescription(workflowDTO.description);
      setIsOnboardingWf(workflowDTO.isOnboardingWF);

      var preflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.preflowNodes.forEach((task) => {
        preflowTasks.push(workflowTemplateDTOToNode(task, preflowTasks));
      })
      setPreflowTasks(preflowTasks);
      var mainflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.mainflowNodes.forEach((task) => {
        mainflowTasks.push(workflowTemplateDTOToNode(task, mainflowTasks));
      })
      setMainflowTasks(mainflowTasks);
      setUpdatingWorkflow(true);
    })
    .catch((error) => {      
      setErrored(true);
      setError("Failed to retrieve workflow template");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function workflowTemplateDTOToNode(node: WorkflowTemplateNodeDTO, nodeList: WorkflowTemplateNode[]) {  
    console.log("TEST123:", node.daysUntilDue)
    var result: WorkflowTemplateNode = {
      id: node.id,
      taskTemplate: taskTemplates.find(t => t.id == node.taskTemplateId),
      assignee: accountsDirectory.concat([PLACEHOLDER_ONBOARDERS_ACCOUNT, PLACEHOLDER_SUPERVISORS_ACCOUNT]).find(a => a.id == node.assigneeId),
      taskDependencies: nodeList.filter(i => node.dependencyNodeIds.includes(i.id)),
      daysUntilDue: node.daysUntilDue
    }    
    return result;
  }

  useEffect(() => {
    var workflowTemplateId = searchParams.get("id");
    if (workflowTemplateId) {
      void fetchWorkflowTemplate(workflowTemplateId);
    }
  }, []);

  return (
    <div className='w-full relative'>
      {/* Workflow name and other attributes tab */}
      <div className='flex flex-col gap-2 items-center absolute top-4 left-1/2 transform -translate-x-1/2 bg-background p-4 px-6 min-w-[400px] z-1 rounded-full' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
        <form className='flex flex-row gap-2' onSubmit={(event: any) => {event.preventDefault(); void createWorkflowTemplate()}}>
          <Input required className='min-w-[350px]' disabled={loading} placeholder='Enter workflow name...' type="text" value={name} onChange={(event: any) => {setName(event.target.value)}}/>
          <Select required onValueChange={(value: string) => { value === "onboarding-workflow" ? setIsOnboardingWf(true) : setIsOnboardingWf(false);}}>
            <SelectTrigger className='min-w-[150px]'>
              <SelectValue placeholder="Select a workflow type"/>
            </SelectTrigger>
            <SelectContent className="w-full z-99">
              <SelectGroup>
                <SelectItem value='onboarding-workflow'>Onboarding</SelectItem>
                <SelectItem value='not-onboarding-workflow'>Not Onboarding</SelectItem>
              </SelectGroup>
            </SelectContent>
            <Button type='submit' className='bg-blue-500 hover:bg-blue-400'>
              {
                loading &&
                <Spinner/>
              }
              {
                updatingWorkflow ? "Update workflow" : "Create workflow"
              }              
            </Button>
          </Select>
        </form>
      </div>
      {
        loading &&
        <div className='w-full flex flex-row justify-center gap-2 py-100'>
          <Spinner/>
          Loading workflow builder...
        </div>
      }
      {
        !loading &&
        <WorkflowTemplateBuilder taskTemplates={taskTemplates} isOnboardingWorkflow={isOnboardingWf}
          preflowTasks={preflowTasks} setPreflowTasks={setPreflowTasks}
          mainflowTasks={mainflowTasks} setMainflowTasks={setMainflowTasks}
        />            
      }
    </div>
  )
}
