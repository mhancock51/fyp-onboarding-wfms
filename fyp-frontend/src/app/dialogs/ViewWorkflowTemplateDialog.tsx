import { Dialog, DialogContent, DialogFooter, DialogHeader } from '@/components/ui/dialog'
import { DialogDescription, DialogTitle } from '@radix-ui/react-dialog'
import React, { useEffect, useState } from 'react'
import WorkflowTemplateBuilder from '../pages/CreateWorkflowPage/WorkflowTemplateBuilder'
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode'
import { useDispatch, useSelector } from 'react-redux'
import { RootState } from '@/store'
import { SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG } from '@/features/appSlice'
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO'
import Api from '@/api'
import Utils from '@/util'
import { Label } from 'recharts'
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
  const [selectedWorkflowTemplateId, setSelectedWorkflowTemplateId] = useState<string | null>(null);

  // workflow data
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [isOnboardingWf, setIsOnboardingWf] = useState<boolean>(true); 
  
  const [preflowTasks, setPreflowTasks] = useState<WorkflowTemplateNode[]>([]);
  const [mainflowTasks, setMainflowTasks] = useState<WorkflowTemplateNode[]>([]);

  const accountsDirectory = useSelector((state: RootState) => state.app.accountsDirectory);
  const taskTemplates = useSelector((state: RootState) => state.app.taskTemplates);  

  async function fetchWorkflowTemplate(workflowTemplateId: string) {    
    setLoading(true);
    await Api.fetchWorkflowTemplate(workflowTemplateId)
    .then((response) => {      
      var workflowDTO = response.data.data as WorkflowTemplateDTO;
      setName(workflowDTO.name);
      setDescription(workflowDTO.description);
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
    setSelectedWorkflowTemplateId(null);
    setPreflowTasks([]);
    setMainflowTasks([]);
    setStep(0);
  }

  useEffect(() => {
    if (selectedWorkflowTemplateId !== null) {
      void fetchWorkflowTemplate(selectedWorkflowTemplateId);
    } 
  }, [selectedWorkflowTemplateId]);

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className='min-w-[1200px]'>
        <DialogHeader>
          <DialogTitle>View Workflow Template</DialogTitle>
        </DialogHeader>
        {
          step === 0 &&
          <>
            <WorkflowTemplateTable 
              onTemplateSelected={(workflowTemplate: WorkflowTemplateDTO) => {setSelectedWorkflowTemplateId(workflowTemplate.id)}}
              selectedTemplateId={selectedWorkflowTemplateId}
            />
            <DialogFooter>
              <div className='flex flex-row w-full justify-end'>
                <Button disabled={selectedWorkflowTemplateId === null} onClick={() => {setStep(1)}} >
                  View Workflow
                </Button>
              </div>
            </DialogFooter>
          </>
        }
        {
          step === 1 &&
          <>
            <div className='px-2 flex flex-col gap-2'>
              {
                loading &&
                <div className='w-full flex flex-row justify-center gap-2 py-100'>
                  <Spinner/>
                  Loading workflow...
                </div>
              }
              {
                !loading &&
                <>               
                <div className='flex flex-row w-full gap-2 justify-center items-center'>
                  <span>{name}</span>  
                  <Badge className='rounded-full p-2'>{isOnboardingWf ? "ONBOARDING" : "NONONBOARDING"}</Badge>
                </div>      
                <WorkflowTemplateBuilder className='h-[60vh]' taskTemplates={[]} 
                  isOnboardingWorkflow={isOnboardingWf} 
                  preflowTasks={preflowTasks} 
                  setPreflowTasks={setPreflowTasks} 
                  mainflowTasks={mainflowTasks} 
                  setMainflowTasks={setMainflowTasks} 
                  isReadonly={true}
                />
                </>
            }
            </div>        
            <DialogFooter>
              <div className='flex flex-row w-full justify-end gap-2'>
                <Button onClick={() => {setStep(0)}} >
                  Back
                </Button>
                <Button onClick={() => {navigate(`/workflows/build?id=${selectedWorkflowTemplateId}`); closeAndClear()}} >
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
