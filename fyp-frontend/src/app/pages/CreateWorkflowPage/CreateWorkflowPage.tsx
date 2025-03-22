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
import WorkflowTemplateNode from '@/models/WorkflowTemplateNode';
import { toast } from 'sonner';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import { CheckedState } from '@radix-ui/react-checkbox';
import { useSearchParams } from 'react-router-dom';
import { WorkflowTemplateNodeDTO } from '@/models/DTOs/WorkflowTemplateNodeDTO';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import { PLACEHOLDER_ONBOARDERS_ACCOUNT, PLACEHOLDER_SUPERVISORS_ACCOUNT } from '@/constants';
import { CreateWorkflowTemplatePayload } from '@/models/payloads/CreateWorkflowTemplatePayload';

export default function CreateWorkflowPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  
  const [loading, setLoading] = useState<boolean>(false); 

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
      preflowTasks: preflowTasks.map((task) => (
        { 
          id: "",
          taskTemplateId: task.taskTemplate?.id ?? "",
          assigneeId: task.assignee?.id ?? "",
          dependencyNodeIds: task.taskDependencies.map((dependency) => dependency.taskTemplate?.id ?? "") ?? [],
          daysUntilDue: task.daysUntilDue
        }
      )),
      mainflowTasks: mainflowTasks.map((task) => (
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
      workflowDTO.preflowTasks.forEach((task) => {
        preflowTasks.push(workflowTemplateDTOToNode(task, preflowTasks));
      })
      setPreflowTasks(preflowTasks);
      var mainflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.mainflowTasks.forEach((task) => {
        mainflowTasks.push(workflowTemplateDTOToNode(task, mainflowTasks));
      })
      setMainflowTasks(mainflowTasks);
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

  useEffect(() => {
    // save preflow tasks
    localStorage.setItem("PREFLOW_TASKS", JSON.stringify(preflowTasks));
  }, [preflowTasks]);

  useEffect(() => {
    // save preflow tasks
    localStorage.setItem("MAINFLOW_TASKS", JSON.stringify(mainflowTasks));
  }, [mainflowTasks]);

  useEffect(() => {
    // save preflow tasks
    localStorage.setItem("WORKFLOW_NAME", name);
  }, [name]);

  useEffect(() => {
    // save preflow tasks
    localStorage.setItem("WORKFLOW_DESCRIPTION", description);
  }, [description]);

  useEffect(() => {
    // save preflow tasks
    localStorage.setItem("IS_ONBOARDING_WORKFLOW", isOnboardingWf ? "true" : "false");
  }, [isOnboardingWf]);

  return (
    <div className='flex flex-col gap-2 w-full items-center'>
      <div className='flex flex-col gap-2 w-1/2'>
        <div className='flex flex-row gap-2 items-center'>
          <div className='flex flex-col gap-1 flex-9'>
            <Label className='flex-3 text-lg'>Workflow Name</Label>
            <Input className='flex-9' disabled={loading}  type="text" value={name} onChange={(event: any) => {setName(event.target.value)}}/>
          </div>
          <div className='flex flex-col gap-1 flex-3 items-start'>
            <Label>Is Onboarding Workflow?</Label>
            <Checkbox checked={isOnboardingWf} disabled={loading} onCheckedChange={(checked: CheckedState) => {setIsOnboardingWf(checked as boolean);}}/>
          </div>
        </div>
        <div className='flex flex-col gap-1'>
          <Label className='flex-3 text-lg'>Description</Label>
          <Textarea className='flex-9' disabled={loading} value={description} onChange={(event: any) => {setDescription(event.target.value);}}/>
        </div>        
      </div>
      <>
      {
        errored &&
        <div className='flex flex-col gap-2'>
          <h2>Failed to load workflow template:</h2>
          <Label>{error}</Label>
        </div>
      }
      {
        loading &&
        <div className='flex flex-row gap-2'>
          <Spinner/>
          <span>Loading...</span>
        </div>
      }       
      <WorkflowTemplateBuilder taskTemplates={taskTemplates} isOnboardingWorkflow={isOnboardingWf}
        preflowTasks={preflowTasks} setPreflowTasks={setPreflowTasks}
        mainflowTasks={mainflowTasks} setMainflowTasks={setMainflowTasks}
      />
      </>
      <Button className='mx-2' onClick={createWorkflowTemplate}>Create Workflow Template</Button>
    </div>
  )
}
