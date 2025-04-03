import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader } from '@/components/ui/dialog'
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import React, { SetStateAction, useEffect, useState } from 'react'
import { ChecklistTemplateCreationForm, ProjectTemplateCreationForm, ReadDocumentTemplateCreationForm, UploadDocumentTemplateCreationForm } from './taskTypeTemplateForms';
import { ChecklistTaskTemplate } from '@/models/tasks/ChecklistTaskTemplate';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import { ReadDocumentTaskTemplate } from '@/models/tasks/ReadDocumentTaskTemplate';
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import ChecklistForm from '@/components/ChecklistForm';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<SetStateAction<boolean>>;
  taskTemplate: TaskTemplate;
}

export default function UpdateTaskTemplateDialog(props: Props) {
  const [description, setDescription] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [hasActiveInstances, setHasActiveInstance] = useState<boolean>(false);
  const [updatedData, setUpdatedData] = useState<ChecklistTaskTemplate | ProjectTaskTemplate | FileUploadTaskTemplate | ReadDocumentTaskTemplate | null>(null);

  function closeAndClear() {
    props.setOpen(false);
  }

  
  function updateTaskTypeData(data: ChecklistTaskTemplate | ProjectTaskTemplate | FileUploadTaskTemplate | ReadDocumentTaskTemplate) {
    setUpdatedData(data);
  }
  
  async function fetchHasActiveInstances() {
    setLoading(true);
    await Api.taskTemplates.fetchTemplateHasActiveInstances(props.taskTemplate.id)
    .then((response: AxiosResponse<HTTPresponse<boolean, string>>) => {
      setHasActiveInstance(response.data.data);
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
    setDescription(props.taskTemplate.description);
    void fetchHasActiveInstances();
  }, [props.taskTemplate]);
  
  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="min-w-[750px]">
        <DialogHeader>
          <h1>Update Task Template {hasActiveInstances ? "(has active instances)" : ""}</h1>
        </DialogHeader>
        <form className='flex flex-col gap-3'>
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
          <Separator/>
          {
            props.taskTemplate.taskTypeId === "checklist" &&
            <ChecklistTemplateCreationForm 
              restrictProperties={hasActiveInstances}
              hideNavButtons={true}
              initialTaskData={props.taskTemplate.taskTypeData as ChecklistTaskTemplate} 
              updateTaskTypeData={updateTaskTypeData} 
              backButtonClick={() => {}}
            />            
          }   
          {
            props.taskTemplate.taskTypeId === "read-document" &&
            <ReadDocumentTemplateCreationForm 
              restrictProperties={hasActiveInstances}
              hideNavButtons={true}
              initialTaskData={props.taskTemplate.taskTypeData as ReadDocumentTaskTemplate}
              updateTaskTypeData={updateTaskTypeData} 
              backButtonClick={() => {}}
            />
          }     
          {
            props.taskTemplate.taskTypeId === "upload-document" &&
            <UploadDocumentTemplateCreationForm 
              restrictProperties={hasActiveInstances}
              hideNavButtons={true}
              initialTaskData={props.taskTemplate.taskTypeData as FileUploadTaskTemplate}
              updateTaskTypeData={updateTaskTypeData} 
              backButtonClick={() => {}}
            />
          }
          {
            props.taskTemplate.taskTypeId === "project-task" &&
            <ProjectTemplateCreationForm 
              restrictProperties={hasActiveInstances}
              hideNavButtons={false}
              initialTaskData={props.taskTemplate.taskTypeData as ProjectTaskTemplate}
              updateTaskTypeData={updateTaskTypeData} 
              backButtonClick={() => {}}
            />
          }
          <DialogFooter>
            <Button type="submit">
              {
                loading &&
                <Spinner className='text-primary-foreground'/>
              }
              Update task template
            </Button>
          </DialogFooter>
        </form>      
      </DialogContent>
    </Dialog>
  )
}

export function UpdateChecklistDataForm(props: {initialTaskData: ChecklistTaskTemplate, restrictProperties: boolean}) {
  const [items, setItems] = useState<string[]>([""]);

  useEffect(() => {
    setItems(props.initialTaskData.items);
  }, [props.initialTaskData]);

  return (
    <div>
      <ChecklistForm items={items} setItems={setItems}/>
    </div>
  )
}
