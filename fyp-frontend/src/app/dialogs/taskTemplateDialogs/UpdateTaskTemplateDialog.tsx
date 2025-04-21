import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader } from '@/components/ui/dialog'
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import React, { SetStateAction, useEffect, useState } from 'react'
import { ChecklistTemplateCreationForm, ProjectTemplateCreationForm, ReadDocumentTemplateCreationForm, UploadDocumentTemplateCreationForm } from './TaskTypeTemplateForms';
import { ChecklistTaskTemplate } from '@/models/tasks/ChecklistTaskTemplate';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import { ReadDocumentTaskTemplate } from '@/models/tasks/ReadDocumentTaskTemplate';
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import { toast } from 'sonner';
import TaskTemplateView from '@/components/TaskTemplateView';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';
import { SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG, SET_SELECTED_TASK_TEMPLATE, SET_TASK_TEMPLATES } from '@/features/appSlice';

interface Props {
  fetchTaskTemplates?: () => Promise<void>;
}

export default function UpdateTaskTemplateDialog(props: Props) {
  const dispatch = useDispatch();
  const open = useSelector((state: RootState) => state.app.openUpdateTaskTemplateDialog);
  const taskTemplate = useSelector((state: RootState) => state.app.selectedTaskTemplate);

  const [step, setStep] = useState<number>(0);
  const [description, setDescription] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [hasActiveInstances, setHasActiveInstance] = useState<boolean>(false);
  const [updatedData, setUpdatedData] = useState<ChecklistTaskTemplate | ProjectTaskTemplate | FileUploadTaskTemplate | ReadDocumentTaskTemplate | null>(null);
  
  const [fetchedHasActiveInstances, setFetchedHasActiveInstances] = useState<boolean>(false);

  function closeAndClear() {
    dispatch(SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG(false));   
    dispatch(SET_SELECTED_TASK_TEMPLATE(null)); 
    setStep(0);
    setDescription("");
    setUpdatedData(null);
  }

  
  function updateTaskTypeData(data: ChecklistTaskTemplate | ProjectTaskTemplate | FileUploadTaskTemplate | ReadDocumentTaskTemplate) {
    if (step !== 1) {
      console.error("Illegal condition met");    
    }
    else {
      setUpdatedData(data);
      setStep(2);
    }
  }

  async function fetchTaskTemplates() {
    setLoading(true);
    Api.taskTemplates.fetchAllTaskTemplates()
    .then((response) => {
      dispatch(SET_TASK_TEMPLATES(response.data.data));      
    })
    .catch((error) => {      
    })
    .finally(() => {
      setLoading(false);     
    }) 
  }
  
  async function updateTaskTemplate() {    
    if (taskTemplate === null) return;

    setLoading(true);
    await Api.taskTemplates.updateTemplate(taskTemplate.id, description, updatedData)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success("Task template updated successfully");
      void fetchTaskTemplates();
      closeAndClear();
    })
    .catch((error) => {
      toast.error("Failed to update task template");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function fetchHasActiveInstances() {
    if (taskTemplate === null) return;

    setLoading(true);
    await Api.taskTemplates.fetchTemplateHasActiveInstances(taskTemplate.id)
    .then((response: AxiosResponse<HTTPresponse<boolean, string>>) => {
      setHasActiveInstance(response.data.data);
      setFetchedHasActiveInstances(true);
    })
    .catch(() => {
      // assume its true
      setHasActiveInstance(true);
    })
    .finally(() => {
      setLoading(false);
    })
  }

  useEffect(() => {
    if (taskTemplate === null) return;
    setFetchedHasActiveInstances(false);
    setDescription(taskTemplate.description);
    setUpdatedData(null);
    void fetchHasActiveInstances();
  }, [taskTemplate, open]);
  
  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="min-w-[750px]">
        <DialogHeader>
          <h1>
            {
              step !== 2 ? (
                <>Update Task Template {hasActiveInstances ? "(has active instances)" : ""}</>
              ) : (
                <>Review Changes</>
              )
            }
          </h1>
        </DialogHeader>
        {
          step === 0 &&
          <form className='flex flex-col gap-3' onSubmit={(event: any) => {event.preventDefault(); setStep(1)}}>
            <div className='grid grid-cols-4'>
              <Label>Task Template Name</Label>
              <Label className='col-span-3 font-normal'>{taskTemplate?.name}</Label>                        
            </div>
            <div className='grid grid-cols-4'>
              <Label>Description</Label>
              <Textarea className='col-span-3 font-normal' value={description} onChange={(event: any) => {setDescription(event.target.value);}}/>
            </div>
            <div className='grid grid-cols-4'>
              <Label>Task Type</Label>
              <Label className='col-span-3 font-normal'>{taskTemplate?.taskType.taskName}</Label>                        
            </div>
            <div className='grid grid-cols-4'>
              <Label>Has Active Instances</Label>              
              <Label className='col-span-3 font-normal'>{hasActiveInstances ? "Yes" : "No"}</Label>                        
            </div>
            <div className='grid grid-cols-4'>
              <Label>Last Modified</Label>                   
              <Label className='col-span-3 font-normal'>
                {
                  taskTemplate?.lastModifiedTimestamp !== null && taskTemplate?.lastModifiedTimestamp !== undefined ? (
                    <>
                    {new Date(taskTemplate.lastModifiedTimestamp).toLocaleTimeString()} {new Date(taskTemplate?.lastModifiedTimestamp).toLocaleDateString()}
                    </>
                  ) : (
                    "Never"
                  )
                }
              </Label>                        
            </div>
            <DialogFooter>
              <Button type="submit" disabled={loading || !fetchedHasActiveInstances}>
                {
                  loading &&
                  <Spinner className="text-primary-foreground"/>
                }
                Next
              </Button>
            </DialogFooter>
          </form>      
        }          
        {
          step === 1 &&
          taskTemplate?.taskTypeId === "checklist" &&
          <ChecklistTemplateCreationForm 
            restrictInputs={hasActiveInstances}
            initialTaskData={taskTemplate?.taskTypeData as ChecklistTaskTemplate} 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}
          />            
        }   
        {
          step === 1 &&
          taskTemplate?.taskTypeId === "read-document" &&
          <ReadDocumentTemplateCreationForm 
            restrictInputs={hasActiveInstances}
            initialTaskData={taskTemplate?.taskTypeData as ReadDocumentTaskTemplate}
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}
          />
        }     
        {
          step === 1 &&
          taskTemplate?.taskTypeId === "upload-document" &&
          <UploadDocumentTemplateCreationForm 
            restrictInputs={hasActiveInstances}                        
            initialTaskData={taskTemplate?.taskTypeData as FileUploadTaskTemplate}
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}
          />
        }
        {
          step === 1 &&
          taskTemplate?.taskTypeId === "project-task" &&
          <ProjectTemplateCreationForm 
            restrictInputs={hasActiveInstances}            
            initialTaskData={taskTemplate?.taskTypeData as ProjectTaskTemplate}
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}
          />
        }
        {
          step === 2 &&
          <form className='flex flex-col gap-3' onSubmit={(event: any) => {event.preventDefault(); void updateTaskTemplate()}}>         
            {
              updatedData !== null && taskTemplate !== null &&
              <TaskTemplateView taskTemplate={{...taskTemplate, taskTypeData: updatedData}}/>           
            }
            <DialogFooter>
              <Button type='button' onClick={() => {setStep(1)}}>Back</Button>
              <Button type="submit">
                {
                  loading &&
                  <Spinner className="text-primary-foreground"/>
                }
                Update Template
              </Button>
            </DialogFooter>
          </form>
        }
      </DialogContent>
    </Dialog>
  )
}
