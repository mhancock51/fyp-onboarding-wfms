import Api from '@/api';
import { MultiSelect } from '@/components/multi-select';
import TaskTypeLookup from '@/components/TaskTypeLookup';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { HoverCard, HoverCardContent, HoverCardTrigger } from '@/components/ui/hover-card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import { TEMPLATE_ACCOUNTS } from '@/constants';
import { ChecklistTaskTemplate } from '@/models/tasks/ChecklistTaskTemplate';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import ProjectObjective from '@/models/tasks/ProjectObjective';
import ProjectSupportLink from '@/models/tasks/ProjectSupportLink';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';
import { ReadDocumentTaskTemplate } from '@/models/tasks/ReadDocumentTaskTemplate';
import TaskType from '@/models/tasks/TaskType';
import { RootState } from '@/store';
import { CheckedState } from '@radix-ui/react-checkbox';
import { DialogDescription } from '@radix-ui/react-dialog';
import { ExternalLink, Trash2 } from 'lucide-react';
import React, { useState } from 'react'
import { useSelector } from 'react-redux';
import { toast } from 'sonner';

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
    await Api.createTaskTemplate(name, description, taskType?.id ?? "", taskTypeData)
    .then((response) => {
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
          step === 1 && taskType?.id === "checklist" &&
          <ChecklistTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData}
            backButtonClick={() => { setStep(0)}}
          />
        }
        {
          step === 1 && taskType?.id === "read-document" &&
          <ReadDocumentTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData}
            backButtonClick={() => {setStep(0)}}            
          />
        }
        {
          step === 1 && taskType?.id === "upload-document" &&
          <UploadDocumentTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}     
          />
        }
        {
          step === 1 && taskType?.id === "project-task" &&
          <ProjectTemplateCreationForm 
            updateTaskTypeData={updateTaskTypeData} 
            backButtonClick={() => {setStep(0)}}
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
                  <Spinner/>
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

function ChecklistTemplateCreationForm(props: { updateTaskTypeData: (data: any) => void; backButtonClick: () => void;}) {
  const [items, setItems] = useState<string[]>([""]);

  function submitChecklist() {
    if (items.length === 0) {
      toast.warning("Please add a checklist item");
      return;
    }
    var data: ChecklistTaskTemplate = {
      id: '',
      taskTemplateId: '',
      items: items
    }
    props.updateTaskTypeData(data);
  }

  function addEmptyItem() {
    setItems((prevState) => ([
      ...items, ""
    ]));
  }

  function deleteItem(index: number) {
    setItems((prevState) => (prevState.filter((_, i) => (i !== index))))
  }

  function updateItem(item: string, index: number) {
    var updatedItems = [...items];
    updatedItems[index] = item;
    setItems(updatedItems);
  }

  return (
    <form className="grid gap-4 py-4" onSubmit={(event: any) => { event.preventDefault(); submitChecklist();}}>
      <div className='max-h-150 overflow-y-auto grid grid-col gap-4'>
        {
          items.map((item, index) => (
            <div className='flex flex-row justify-between items-center'>
              <Input required placeholder='Enter description of task...' className="col-span-3" value={item} onChange={(event: any) => {updateItem(event.target.value, index);}}/>
              <Button className='my-1 mx-2' onClick={() => {deleteItem(index);}} variant={"destructive"}><Trash2/></Button>
            </div>
          ))
        }
      </div>
      <div className="grid grid-row items-center gap-4">
        <Button onClick={addEmptyItem}>Add Item</Button>
      </div>
      <DialogFooter>
        <Button type='button' onClick={props.backButtonClick}>Back</Button>
        <Button type="submit">Next</Button>
      </DialogFooter>
    </form>
  )
}

function ReadDocumentTemplateCreationForm(props: { updateTaskTypeData: (data: any) => void; backButtonClick: () => void;}) {
  const [documentLink, setDocumentLink] = useState<string>("");
  const [documentName, setDocumentName] = useState<string>("");
  const [checkboxLabel, setCheckboxLabel] =  useState<string>("");;

  function submitReadDocTask() {
    if (documentLink === "") return;
    if (checkboxLabel === "") return;

    const data: ReadDocumentTaskTemplate = {
      id: '',
      taskTemplateId: '',
      documentName: documentName,
      documentUrl: documentLink,
      checkBoxLabel: checkboxLabel
    }
    props.updateTaskTypeData(data);
  }

  return (
    <form className="grid gap-4 py-4" onSubmit={(event: any) => { event.preventDefault(); submitReadDocTask();}}>
      <div className="grid grid-cols-4 items-center gap-4">
        <Label htmlFor="name" className="text-right">Document Link</Label>
        <Input required className="col-span-3" value={documentLink} onChange={(event: any) => {setDocumentLink(event.target.value);}}/>
      </div>  
      <div className="grid grid-cols-4 items-center gap-4">
        <Label htmlFor="name" className="text-right">Document Name</Label>
        <Input required className="col-span-3" value={documentName} onChange={(event: any) => {setDocumentName(event.target.value);}}/>
      </div>  
      <div className="grid grid-cols-4 items-center gap-4">
        <Label htmlFor="name" className="text-right">Checkbox Label</Label>
        <Input required className="col-span-3" placeholder='e.g. I have read coding guidelines...'
          value={checkboxLabel} onChange={(event: any) => {setCheckboxLabel(event.target.value);}}
        />
      </div> 
      <DialogFooter>
      <Button type='button' onClick={props.backButtonClick}>Back</Button>
        <Button type="submit">Next</Button>
      </DialogFooter> 
    </form>
  )
}

function UploadDocumentTemplateCreationForm(props: { updateTaskTypeData: (data: any) => void; backButtonClick: () => void;}) {
  const [documentName, setDocumentName] = useState<string>("");
  const [fileExtensions, setFileExtensions] = useState<string[]>([]);
  const [accessAccountIds, setAccessAccountIds] = useState<string[]>([]);

  const accountDirectories = useSelector((state: RootState) => state.app.accountsDirectory);

  function submitUploadDocTask() {
    if (fileExtensions.length === 0) {
      toast.warning("Please select at least one file extension");
      return;
    }
    const data: FileUploadTaskTemplate = {
      id: '',
      taskTemplateId: '',
      supportedDocumentType: fileExtensions.join(";"),
      documentName: documentName,
      accessAccountIds: accessAccountIds
    }
    props.updateTaskTypeData(data);
  }

  return (
    <form className="grid gap-4 py-4" onSubmit={(event: any) => { event.preventDefault(); submitUploadDocTask();}}>
      <div className="grid grid-cols-4 items-center gap-4">
        <Label htmlFor="name" className="text-right">Document Name</Label>
        <Input required className="col-span-3" value={documentName} onChange={(event: any) => {setDocumentName(event.target.value);}}/>
      </div>
      <div className="grid grid-cols-4 items-center gap-4">
        <Label htmlFor="name" className="text-right">Support Document Types</Label>
        <MultiSelect           
          className='w-100'
          variant={"inverted"}
          options={[
            { label: ".pdf", value: ".pdf"},
            { label: ".png", value: ".png"},
            { label: ".jpeg", value: ".jpeg"},
            { label: ".docx", value: ".odt"}
          ]} 
          onValueChange={(value: string[]) => { setFileExtensions(value);}}
        />        
      </div>
      <div className="grid grid-cols-4 items-center gap-4">
        <Label htmlFor="name" className="text-right">Account Access</Label>
        <MultiSelect           
          className='w-100'
          variant={"inverted"}
          options={TEMPLATE_ACCOUNTS.concat(accountDirectories).map((account) => (
            {
              value: account.id,
              label: `${account.displayName} ${account.departmentName !== "" ? `(${account.departmentName})` : ""}`
            }
          ))} 
          onValueChange={(value: string[]) => { setAccessAccountIds(value)}}
        />        
      </div>
      <DialogFooter className='flex flex-row justify-between'> 
        <Button type='button' onClick={props.backButtonClick}>Back</Button>
        <Button type="submit">Next</Button>
      </DialogFooter> 
    </form>
  )
}

function ProjectTemplateCreationForm(props: { updateTaskTypeData: (data: any) => void; backButtonClick: () => void;}) {
  const [substep, setSubstep] = useState<number>(0);

  const [brief, setBrief] = useState<string>("");
  const [deliverable, setDeliverable] = useState<string>("");
  const [objectives, setObjectives] = useState<ProjectObjective[]>([]);
  const [skills, setSkills] = useState<string[]>([]);
  const [supportLinks, setSupportLinks] = useState<ProjectSupportLink[]>([]);
  
  function submitProject() {
    var data: ProjectTaskTemplate = {
      id: '',
      taskTemplateId: '',
      brief: brief,
      deliverable: deliverable,
      objectives: objectives,
      skills: skills,
      supportLinks: supportLinks
    }
    props.updateTaskTypeData(data);
  }

  function addEmptyObjective() {
    setObjectives((prevState) => ([
      ...prevState, { id: prevState.length.toString(), objective: "", required: false}
    ]));
  }

  function updateObjectiveText(objectiveText: string, index: number) {
    var updatedItems = [...objectives];
    updatedItems[index].objective = objectiveText;
    setObjectives(updatedItems);
  }

  function updateObjectiveRequirement(required: boolean, index: number) {
    var updatedItems = [...objectives];
    updatedItems[index].required = required;
    setObjectives(updatedItems);
  }

  function deleteObjective(index: number) {
    setObjectives((prevState) => (prevState.filter((_, i) => (i !== index))))
  }

  function addEmptySupprtLink() {
    setSupportLinks((prevState) => (
      [...prevState, { id: prevState.length.toString(), description: "", linkLabel: "", link: ""}]
    ));
  }

  function updateLinkLabel(label: string, index: number) {
    var updatedItems = [...supportLinks];
    updatedItems[index].linkLabel = label;
    setSupportLinks(updatedItems);
  }

  function updateLinkDescription(description: string, index: number) {
    var updatedItems = [...supportLinks];
    updatedItems[index].description = description;
    setSupportLinks(updatedItems);
  }

  function updateLink(link: string, index: number) {
    var updatedItems = [...supportLinks];
    updatedItems[index].link = link;
    setSupportLinks(updatedItems);
  }

  return (
    <>
    {
      substep === 0 &&
      <form className="flex flex-col gap-4 py-4" onSubmit={(event: any) => { event.preventDefault(); setSubstep(1);}}>
        <div className="flex flex-col gap-2">
          <Label htmlFor="name" >Project Brief</Label>
          <Textarea required className='col-span-3' value={brief} onChange={(event: any) => {setBrief(event.target.value);}}/>
        </div>
        <div className="flex flex-col gap-2">
          <Label htmlFor="name" >Deliverable</Label>
          <Input type='text' required className='col-span-3' value={deliverable} onChange={(event: any) => {setDeliverable(event.target.value);}}/>
        </div>
        <div className='flex flex-col gap-2 w-full'>
          <Label>Skills ({skills.length})</Label>
          <Label className='font-normal'>Awarded to user on completion of the project</Label>
          <MultiSelect options={[
            { label: "Frontend", value: "frontend"},
            { label: "Backend", value: "backend"},
            { label: "React.js", value: "reactjs"},

          ]} onValueChange={(value: string[]) => {setSkills(value);}}/>
        </div>
        <div className='flex flex-col gap-2 w-full'>
          <Label>Resources</Label>
        </div>
        <div className='flex flex-col gap-2 w-full'>
          <Label htmlFor="name" >Objectives ({objectives.length})</Label>
          <div className='max-h-150 overflow-y-auto flex flex-col gap-2 w-full p-2 my-2 rounded-input bg-sidebar rounded-[15px]'>
            {
              objectives.length === 0 &&
              <div className='w-full text-center'>No Objectives</div>
            }
            {
              objectives.map((objective, index) => (
                <div key={index} className='flex flex-row justify-between items-center w-full gap-2'>
                  <Input required placeholder='Enter objective...' className="flex-11 bg-background" value={objective.objective} onChange={(event: any) => {updateObjectiveText(event.target.value, index);}}/>
                  <HoverCard>
                    <HoverCardTrigger>
                      <Checkbox checked={objective.required} onCheckedChange={(checked: CheckedState) => {updateObjectiveRequirement(checked as boolean, index)}}/>
                    </HoverCardTrigger>
                    <HoverCardContent className='p-2 my-1 w-[175px] flex flex-row justify-center text-center'>
                      <Label className='text-sm'>Required to complete project?</Label>                    
                    </HoverCardContent>
                  </HoverCard>
                  <Button className='my-1 mx-2 flex-1' onClick={() => {deleteObjective(index);}} variant={"destructive"}><Trash2/></Button>
                </div>
              ))
            }
          </div>
          <div className="grid grid-row items-center gap-4">
            <Button onClick={addEmptyObjective}>Add Objective</Button>
          </div>
        </div>
        <DialogFooter>
          <Button type='button' onClick={(props.backButtonClick)}>Back</Button>
          <Button type="submit">Next</Button>
        </DialogFooter>
      </form>
    }
    {
      substep === 1 &&
      <form className="flex flex-col gap-4 py-4" onSubmit={(event: any) => { event.preventDefault(); submitProject();}}>
        <div className='flex flex-col gap-2 w-full max-h-[50vh] overflow-y-auto'>
          <Label htmlFor="name" >Supporting Links ({supportLinks.length})</Label>
          <div className='max-h-150 overflow-y-auto flex flex-col gap-2 w-full p-2 my-2 rounded-input'>
            {
              supportLinks.length === 0 &&
              <div className='w-full text-center'>No Links</div>
            }
            {
              supportLinks.map((link, index) => (
                <div key={index} className='flex flex-col justify-between items-center w-full gap-2 bg-sidebar rounded-[15px] p-1'>              
                  <Badge className={`p-2 rounded-full cursor-pointer min-w-[250px] ${link.linkLabel === "" ? "invisible" : "visible"}`}
                    onClick={() => {window.open(link.link, '_blank');}}
                  >
                    {link.linkLabel} <ExternalLink size={50}/>
                  </Badge>                  
                  <Input required placeholder='Enter link label' className="bg-background" value={link.linkLabel} onChange={(event: any) => {updateLinkLabel(event.target.value, index)}}/>
                  <Textarea required placeholder='Enter description...' className="bg-background" value={link.description} onChange={(event: any) => {updateLinkDescription(event.target.value, index);}}/>
                  <Input required placeholder='Enter link...' className="bg-background" value={link.link} onChange={(event: any) => {updateLink(event.target.value, index);}}/>
                </div>
              ))
            }
          </div>
          <div className="grid grid-row items-center gap-4">
            <Button onClick={addEmptySupprtLink}>Add Support Link</Button>
          </div>
        </div>
        <DialogFooter>
          <Button type='button' onClick={() => {setSubstep(0);}}>Back</Button>
          <Button type="submit">Next</Button>
        </DialogFooter>
      </form>
    }
    </>
  )
}