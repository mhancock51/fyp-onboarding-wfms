import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader } from '@/components/ui/dialog'
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import TaskTemplate from '@/models/tasks/TaskTemplate';
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

interface Props {
  open: boolean;
  setOpen: React.Dispatch<SetStateAction<boolean>>;
  taskTemplate: TaskTemplate;
  fetchTaskTemplates: () => Promise<void>;
}

export default function UpdateTaskTemplateDialog(props: Props) {
  const [step, setStep] = useState<number>(0);
  const [description, setDescription] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [hasActiveInstances, setHasActiveInstance] = useState<boolean>(false);
  const [updatedData, setUpdatedData] = useState<ChecklistTaskTemplate | ProjectTaskTemplate | FileUploadTaskTemplate | ReadDocumentTaskTemplate | null>(null);
  
  const [fetchedHasActiveInstances, setFetchedHasActiveInstances] = useState<boolean>(false);

  function closeAndClear() {
    props.setOpen(false);
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
  
  async function updateTaskTemplate() {    
    setLoading(true);
    await Api.taskTemplates.updateTemplate(props.taskTemplate.id, description, updatedData)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success("Task template updated successfully");
      void props.fetchTaskTemplates();
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
    setLoading(true);
    await Api.taskTemplates.fetchTemplateHasActiveInstances(props.taskTemplate.id)
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
    setFetchedHasActiveInstances(false);
    setDescription(props.taskTemplate.description);
    setUpdatedData(null);
    void fetchHasActiveInstances();
  }, [props.taskTemplate]);
  
  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="min-w-[750px]">
        <DialogHeader>
          <h1>Update Task Template {hasActiveInstances ? "(has active instances)" : ""}</h1>
        </DialogHeader>
        {
          step === 0 &&
          <form className='flex flex-col gap-3' onSubmit={(event: any) => {event.preventDefault(); setStep(1)}}>
            <div className='grid grid-cols-4'>
              <Label>Task Template Name</Label>
              <Label className='col-span-3 font-normal'>{props.taskTemplate.name}</Label>                        
            </div>
            <div className='grid grid-cols-4'>
              <Label>Description</Label>
              <Textarea className='col-span-3 font-normal' value={description} onChange={(event: any) => {setDescription(event.target.value);}}/>
            </div>
            <div className='grid grid-cols-4'>
              <Label>Task Type</Label>
              <Label className='col-span-3 font-normal'>{props.taskTemplate.taskType.taskName}</Label>                        
            </div>
            <div className='grid grid-cols-4'>
              <Label>Has Active Instances</Label>              <Label className='col-span-3 font-normal'>{hasActiveInstances ? "yes" : "no"}</Label>                        
            </div>
            <DialogFooter>
              <Button type="submit" disabled={loading || !fetchedHasActiveInstances}>
                {
                  loading &&
                  <Spinner className='text-primary-foreground'/>
                }
                Next
              </Button>
            </DialogFooter>
          </form>      
        }          
        {
          step === 1 &&
          props.taskTemplate.taskTypeId === "checklist" &&
          <ChecklistTemplateCreationForm 
            restrictInputs={hasActiveInstances}
            initialTaskData={props.taskTemplate.taskTypeData as ChecklistTaskTemplate} 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {}}
          />            
        }   
        {
          step === 1 &&
          props.taskTemplate.taskTypeId === "read-document" &&
          <ReadDocumentTemplateCreationForm 
            restrictInputs={hasActiveInstances}
            initialTaskData={props.taskTemplate.taskTypeData as ReadDocumentTaskTemplate}
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {}}
          />
        }     
        {
          step === 1 &&
          props.taskTemplate.taskTypeId === "upload-document" &&
          <UploadDocumentTemplateCreationForm 
            restrictInputs={hasActiveInstances}                        
            initialTaskData={props.taskTemplate.taskTypeData as FileUploadTaskTemplate}
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {}}
          />
        }
        {
          step === 1 &&
          props.taskTemplate.taskTypeId === "project-task" &&
          <ProjectTemplateCreationForm 
            restrictInputs={hasActiveInstances}            
            initialTaskData={props.taskTemplate.taskTypeData as ProjectTaskTemplate}
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {}}
          />
        }
        {
          step === 2 &&
          <form className='flex flex-col gap-3' onSubmit={(event: any) => {event.preventDefault(); void updateTaskTemplate()}}>
            <DialogFooter>
              <Button type="submit">
                {
                  loading &&
                  <Spinner className='text-primary-foreground'/>
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
