import Api from '@/api';
import TaskTypeLookup from '@/components/TaskTypeLookup';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import TaskType from '@/models/tasks/TaskType';
import { DialogDescription } from '@radix-ui/react-dialog';
import { useState } from 'react';
import { toast } from 'sonner';
import { ChecklistTemplateCreationForm, FeedbackTemplateCreationForm, ProjectTemplateCreationForm, ReadDocumentTemplateCreationForm, UploadDocumentTemplateCreationForm } from './TaskTypeTemplateForms';
import { TASK_TYPE_IDS } from '@/constants';

interface Props {
  open: boolean;
  setOpenDialog: (open: boolean) => void;
}

export default function CreateTaskTemplateDialog(props: Props) {
  const [step, setStep] = useState<number>(0);
  const [taskType, setTaskType] = useState<TaskType | null>(null);
  const [name, setName] = useState<string>("");
  const [description, setDescription] = useState<string>("");

  const [taskTypeData, setTaskTypeData] = useState<any | null>(null);
  
  const [loading, setLoading] = useState<boolean>(false);


  function closeAndClear() {
    setName("");
    setDescription("");
    setTaskType(null);
    setTaskTypeData(null);
    setStep(0);
    props.setOpenDialog(false);
  }

  function updateTaskTypeData(data: any) {
    setTaskTypeData(data);
    setStep(2);
  }

  async function createTaskTemplate() {
    setLoading(true);
    await Api.taskTemplates.createTaskTemplate(name, description, taskType?.id ?? "", taskTypeData)
    .then(() => {
      setLoading(false);
      toast("Successfully created task");
      closeAndClear();
    })
    .catch((error) => {
      console.error(error);
      setLoading(false);
      const errorMessage = error.response.data.error;
      if (errorMessage === undefined) {
        toast.error(`Failed to create task template`);
      }
      else {
        toast.error(`Failed to create task template: ${errorMessage}`);
      }    
    })
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Create a Task Template</DialogTitle> 
          {
            step === 2 &&
            <DialogDescription>Confirm task details</DialogDescription>
          }         
        </DialogHeader>
        {
          step === 0 &&
          <form className="grid gap-4 py-4" onSubmit={(event: any) => { event.preventDefault(); if (taskType !== null) setStep(1);}}>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Name</Label>
              <Input required className="col-span-3" value={name} onChange={(event: any) => { setName(event.target.value);}} />
            </div>   
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Description</Label>
              <Textarea required className="col-span-3" value={description} onChange={(event: any) => { setDescription(event.target.value);}} />
            </div>  
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Task Type</Label>
              <TaskTypeLookup setTaskType={setTaskType} value={taskType?.id}/>
            </div>       
            <DialogFooter>
              <Button type="submit">Next</Button>
            </DialogFooter>
          </form>
        }
        {
          step === 1 && taskType?.id === TASK_TYPE_IDS.CHECKLIST &&
          <ChecklistTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData}
            backButtonClick={() => { setStep(0)}}
            restrictInputs={false}
          />
        }
        {
          step === 1 && taskType?.id === TASK_TYPE_IDS.READ_DOCUMENT &&
          <ReadDocumentTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData}
            backButtonClick={() => {setStep(0)}}    
            restrictInputs={false}        
          />
        }
        {
          step === 1 && taskType?.id === TASK_TYPE_IDS.UPLOAD_DOCUMENT &&
          <UploadDocumentTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}    
            restrictInputs={false} 
          />
        }
        {
          step === 1 && taskType?.id === TASK_TYPE_IDS.PROJECT_TASK &&
          <ProjectTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData}
            backButtonClick={() => { setStep(0); } } 
            restrictInputs={false}          
          />
        }
        {
          step === 1 && taskType?.id === TASK_TYPE_IDS.FEEDBACK_TASK &&
          <FeedbackTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => { setStep(0);}}
            restrictInputs={false}
          />          
        }
        {
          step === 1 && taskType?.id === TASK_TYPE_IDS.DECISION_TASK &&
          <FeedbackTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => { setStep(0);}}
            restrictInputs={false}
          />
        }
        {
          step === 2 &&
          <form className="grid gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); void createTaskTemplate();}}>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Name</Label>
              <Label  className="col-span-3">{name}</Label>
            </div>   
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Description</Label>
              <Label className="col-span-3">{description}</Label>
            </div>  
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="name" className="text-right">Task Type</Label>
              <Label className="col-span-3">{taskType?.taskName}</Label>
            </div>  
            <DialogFooter>
              <Button type='button' onClick={() => {setStep(1);}}>
                Back
              </Button>
              <Button type="submit">
                {
                  loading &&
                  <Spinner className="text-primary-foreground"/>
                }
                Create Task Template
              </Button>
            </DialogFooter>
          </form>
        }
      </DialogContent>
    </Dialog>
  )
}