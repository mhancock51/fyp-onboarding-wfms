import { Dialog, DialogContent, DialogFooter, DialogHeader } from '@/components/ui/dialog'
import { DialogTitle } from '@radix-ui/react-dialog'
import { useEffect, useState } from 'react'
import WorkflowTemplateBuilder from '../pages/CreateWorkflowPage/WorkflowTemplateBuilder'
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode'
import { useDispatch, useSelector } from 'react-redux'
import { RootState } from '@/store'
import { SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG } from '@/features/appSlice'
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO'
import Api from '@/api'
import Utils from '@/util'
import { Spinner } from '@/components/ui/spinner'
import { Button } from '@/components/ui/button'
import { useNavigate } from 'react-router-dom'
import WorkflowTemplateTable from '@/components/Tables/WorkflowTemplateTable'
import { Badge } from '@/components/ui/badge'

export default function ViewWorkflowTemplateDialog() {
  const open = useSelector((state: RootState) => state.app.openViewWorkflowTemplateDialog);
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const [loading, setLoading] = useState<boolean>(false); 
  const [step, setStep] = useState<number>(0); 
  const [selectedWorkflowTemplate, setSelectedWorkflowTemplate] = useState<WorkflowTemplateDTO | null>(null);

  // workflow data
  const [name, setName] = useState<string>("");
  const [isOnboardingWf, setIsOnboardingWf] = useState<boolean>(true); 
  
  const [preflowTasks, setPreflowTasks] = useState<WorkflowTemplateNode[]>([]);
  const [mainflowTasks, setMainflowTasks] = useState<WorkflowTemplateNode[]>([]);

  const accountsDirectory = useSelector((state: RootState) => state.app.accountsDirectory);
  const taskTemplates = useSelector((state: RootState) => state.app.taskTemplates);  

  async function fetchWorkflowTemplate(workflowTemplateId: string) {    
    setLoading(true);
    await Api.workflowTemplates.fetchWorkflowTemplate(workflowTemplateId)
    .then((response) => {      
      var workflowDTO = response.data.data as WorkflowTemplateDTO;
      setName(workflowDTO.name);
      setIsOnboardingWf(workflowDTO.isOnboardingWF);

      var preflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.preflowNodes.forEach((task) => {
        preflowTasks.push(Utils.workflowTemplateDTOToNode(task, preflowTasks, taskTemplates, accountsDirectory));
      })
      setPreflowTasks(preflowTasks);
      var mainflowTasks: WorkflowTemplateNode[] = [];
      workflowDTO.mainflowNodes.forEach((task) => {
        mainflowTasks.push(Utils.workflowTemplateDTOToNode(task, mainflowTasks, taskTemplates, accountsDirectory));
      })
      setMainflowTasks(mainflowTasks);
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function closeAndClear() {
    dispatch(SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG(false));
    setSelectedWorkflowTemplate(null);
    setPreflowTasks([]);
    setMainflowTasks([]);
    setStep(0);
  }

  useEffect(() => {
    if (selectedWorkflowTemplate !== null) {
      void fetchWorkflowTemplate(selectedWorkflowTemplate.id);
    } 
  }, [selectedWorkflowTemplate]);

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className='min-w-[900px]'>
        <DialogHeader>
          <DialogTitle>Workflow Templates</DialogTitle>
        </DialogHeader>
        {
          step === 0 &&
          <>
            <WorkflowTemplateTable 
              onTemplateSelected={(workflowTemplate: WorkflowTemplateDTO) => {setSelectedWorkflowTemplate(workflowTemplate)}}
              selectedTemplate={selectedWorkflowTemplate}
            />
            <DialogFooter>
              <div className='flex flex-row w-full justify-end'>
                <Button disabled={selectedWorkflowTemplate === null} onClick={() => {setStep(1)}} >
                  View Workflow
                </Button>
              </div>
            </DialogFooter>
          </>
        }
        {
          step === 1 &&
          <>
            <div className='p-2 py-3 flex flex-col gap-2'>
              {
                loading &&
                <div className='w-full flex flex-row justify-center gap-2 py-100'>
                  <Spinner/>
                  Loading workflow...
                </div>
              }
              {
                !loading &&
                <div className='w-full relative'>               
                  <div className='flex flex-row gap-4 items-center min-w-[200px] justify-between absolute top-4 left-1/2 transform -translate-x-1/2 bg-background p-2 px-4 min-w-[400px] z-1 rounded-full' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
                    <h1>{name}</h1>  
                    <Badge className='rounded-full px-6 py-2'>{isOnboardingWf ? "ONBOARDING" : "NONONBOARDING"}</Badge>
                  </div>      
                  <WorkflowTemplateBuilder className='h-[60vh]' taskTemplates={[]} 
                    isOnboardingWorkflow={isOnboardingWf} 
                    preflowNodes={preflowTasks} 
                    setPreflowNodes={setPreflowTasks} 
                    mainflowNodes={mainflowTasks} 
                    setMainflowNodes={setMainflowTasks} 
                    isReadonly={true}
                  />
                </div>
            }
            </div>        
            <DialogFooter>
              <div className='flex flex-row w-full justify-end gap-2'>
                <Button onClick={() => {setStep(0)}} >
                  Back
                </Button>
                <Button disabled={selectedWorkflowTemplate?.status === "archived"} onClick={() => {navigate(`/workflows/build?id=${selectedWorkflowTemplate?.id}`); closeAndClear()}} >
                  Update Workflow
                </Button>
              </div>
            </DialogFooter>
          </>
            
        }
      </DialogContent>
    </Dialog>
  )
}
