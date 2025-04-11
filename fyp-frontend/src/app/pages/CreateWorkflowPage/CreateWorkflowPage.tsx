import Api from '@/api';
import WorkflowTemplateBuilder from './WorkflowTemplateBuilder';
import { SetStateAction, useEffect, useState } from 'react';
import { Input } from '@/components/ui/input';
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode';
import { toast } from 'sonner';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import { useSearchParams } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';
import { CreateWorkflowTemplatePayload } from '@/models/payloads/CreateWorkflowTemplatePayload';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Spinner } from '@/components/ui/spinner';
import Utils from '@/util';
import { SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG } from '@/features/appSlice';
import { HoverCard, HoverCardTrigger } from '@/components/ui/hover-card';
import { HoverCardContent } from '@radix-ui/react-hover-card';
import { Label } from '@/components/ui/label';
import { Info } from 'lucide-react';

export default function CreateWorkflowPage() {
  const dispatch = useDispatch();

  const [searchParams, setSearchParams] = useSearchParams();
  
  const [loading, setLoading] = useState<boolean>(false);     
  const [workflowTemplateId, setWorkflowTemplateId] = useState<string | null>(null);

  const [errored, setErrored] = useState<boolean>(false); 
  const [error, setError] = useState<string>("");
  
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [isOnboardingWf, setIsOnboardingWf] = useState<boolean>(true); 
  
  const [preflowNodes, setPreflowNodes] = useState<WorkflowTemplateNode[]>([]);
  const [mainflowNodes, setMainflowNodes] = useState<WorkflowTemplateNode[]>([]);

  const accountsDirectory = useSelector((state: RootState) => state.app.accountsDirectory);
  const taskTemplates = useSelector((state: RootState) => state.app.taskTemplates);

  function buildCreateWorkflowTemplatePayload() {  
    const payload: CreateWorkflowTemplatePayload = {
      id: workflowTemplateId !== null ? workflowTemplateId : "",
      name: name,
      description: description,
      isOnboardingWF: isOnboardingWf,
      preflowNodes: preflowNodes.map((node) => (
        { 
          id: node.id,
          taskTemplateId: node.taskTemplate?.id ?? "",
          assigneeId: node.assignee?.id ?? "",
          dependencyNodeIds: node.taskDependencies.map((dependency) => dependency.id),
          daysUntilDue: node.daysUntilDue,
          accountsToNotify: node.accountsToNotify
        }
      )),
      mainflowNodes: mainflowNodes.map((node) => (
        { 
          id: node.id,
          taskTemplateId: node.taskTemplate?.id ?? "",
          assigneeId: node.assignee?.id ?? "",
          dependencyNodeIds: node.taskDependencies.map((dependency) => dependency.id),
          daysUntilDue: node.daysUntilDue,
          accountsToNotify: node.accountsToNotify
        }
      )),
    }
    return payload;
  }

  async function createWorkflowTemplate() {    
    setLoading(true);
    const payload = buildCreateWorkflowTemplatePayload();
    await Api.workflowTemplates.createWorkflowTemplate(payload)
    .then((response) => {
      toast.success("Successfully created workflow template");
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to create workflow template");
      } 
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function updateWorkflowTemplate() {
    if (workflowTemplateId === null) return;
    
    setLoading(true);
    const payload = buildCreateWorkflowTemplatePayload();
    await Api.workflowTemplates.updateWorkflowTemplate(payload)
    .then((response) => {
      toast.success("Successfully created workflow template");
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to update workflow template");
      }            
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function fetchWorkflowTemplate(workflowTemplateId: string) {    
    setLoading(true);
    await Api.workflowTemplates.fetchWorkflowTemplate(workflowTemplateId)
    .then((response) => {      
      var workflowDTO = response.data.data as WorkflowTemplateDTO;
      setName(workflowDTO.name);
      setDescription(workflowDTO.description);
      setIsOnboardingWf(workflowDTO.isOnboardingWF);

      var preflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.preflowNodes.forEach((task) => {
        preflowTasks.push(Utils.workflowTemplateDTOToNode(task, preflowTasks, taskTemplates, accountsDirectory));
      })
      setPreflowNodes(preflowTasks);
      var mainflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.mainflowNodes.forEach((task) => {
        mainflowTasks.push(Utils.workflowTemplateDTOToNode(task, mainflowTasks, taskTemplates, accountsDirectory));
      })
      setMainflowNodes(mainflowTasks);
      setWorkflowTemplateId(workflowDTO.id);      
    })
    .catch((error) => {      
      setErrored(true);
      setError("Failed to retrieve workflow template");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function handleFormSubmission() {
    if (workflowTemplateId === null) {
      void createWorkflowTemplate();
    }
    else {
      void updateWorkflowTemplate();
    }
  }

  useEffect(() => {
    dispatch(SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG(false));
    var workflowTemplateId = searchParams.get("id");
    if (workflowTemplateId) {
      void fetchWorkflowTemplate(workflowTemplateId);
    }
  }, []);

  return (
    <div className='w-full relative'>      
      <WorkflowTemplateBar
        name={name}
        setName={setName}
        isOnboardingWf={isOnboardingWf}
        setIsOnboardingWf={setIsOnboardingWf}
        workflowTemplateId={workflowTemplateId}
        loading={loading}
        handleFormSubmission={handleFormSubmission}
      />
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
          preflowNodes={preflowNodes} setPreflowNodes={setPreflowNodes}
          mainflowNodes={mainflowNodes} setMainflowNodes={setMainflowNodes}
          isReadonly={false}
          className='h-[96vh]'
        />            
      }      
    </div>
  )
}

interface WorkflowTemplateBarProps {
  name: string;
  setName: React.Dispatch<React.SetStateAction<string>>;
  isOnboardingWf: boolean;
  setIsOnboardingWf: React.Dispatch<React.SetStateAction<boolean>>;
  workflowTemplateId: string | null;
  loading: boolean;
  handleFormSubmission: () => void;
}

function WorkflowTemplateBar(props: WorkflowTemplateBarProps) {
  return (
    <div className='flex flex-col gap-2 items-center absolute top-4 left-1/2 transform -translate-x-1/2 bg-background p-4 px-6 min-w-[400px] z-1 rounded-full' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
      <form className='flex flex-row gap-2' onSubmit={(event: any) => {event.preventDefault(); props.handleFormSubmission();}}>
        <Input required className='min-w-[350px]' placeholder='Enter workflow name...' value={props.name} onChange={(event: any) => {props.setName(event.target.value)}}/>
        <Select required value={props.isOnboardingWf ? "onboarding-workflow" : "not-onboarding-workflow"} 
          onValueChange={(value: string) => { value === "onboarding-workflow" ? props.setIsOnboardingWf(true) : props.setIsOnboardingWf(false);}}
        >
          <SelectTrigger className='min-w-[150px]'>
            <SelectValue placeholder="Select a workflow type"/>
          </SelectTrigger>
          <SelectContent className="w-full z-99">
            <SelectGroup>
              <SelectItem value='onboarding-workflow'>Onboarding</SelectItem>
              <SelectItem value='not-onboarding-workflow'>Not Onboarding</SelectItem>
            </SelectGroup>
          </SelectContent>
          {
            props.workflowTemplateId !== null ? (
              <HoverCard>
                <HoverCardTrigger>
                  <Button type='submit' className='bg-blue-500 hover:bg-blue-400'>
                    { props.loading && <Spinner className='text-primary-foreground'/> }
                    Update Workflow
                  </Button>
                </HoverCardTrigger>
                <HoverCardContent>
                  <div className='p-2 bg-background rounded-xl max-w-[300px] my-2 flex flex-row gap-2 w-full items-center' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
                    <Info size={40}/>
                    <Label className='font-normal text-sm'>Changes won't affect existing instances of this workflow template</Label>
                  </div>
                </HoverCardContent>
              </HoverCard>
            ) : (
              <Button type='submit' className='bg-blue-500 hover:bg-blue-400'>
                { props.loading && <Spinner className='text-primary-foreground'/> }
                Create workflow
              </Button>
            )
          }
        </Select>
      </form>
    </div>
  )
}