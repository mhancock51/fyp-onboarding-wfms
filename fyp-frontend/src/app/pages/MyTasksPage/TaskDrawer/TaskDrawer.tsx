import { Drawer, DrawerClose, DrawerContent, DrawerDescription, DrawerFooter, DrawerHeader, DrawerTitle, DrawerTrigger } from '@/components/ui/drawer'
import TaskInstance from '@/models/TaskInstance'
import TaskTypeBadge from '../TaskTypeBadge';
import { Separator } from '@/components/ui/separator';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { FileUploadTaskTemplate } from '@/models/FileUploadTaskTemplate';
import { ReadDocumentTaskTemplate } from '@/models/ReadDocumentTaskTemplate';
import ChecklistTask from './ChecklistTask';
import { ChecklistTaskInstance } from '@/models/ChecklistTaskInstance';
import { ChecklistTaskTemplate } from '@/models/ChecklistTaskTemplate';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  task: TaskInstance | null;
  fetchTaskInstances: () => Promise<void>;
}

export default function TaskDrawer(props: Props) {  
  return (
    <Drawer direction='right' onClose={() => {props.setOpen(false);}} open={props.open}>
      <DrawerContent className="max-w-[600px] w-full p-2"> {/* Override max width */}
        {
          props.task !== null &&
          <>
          <DrawerHeader className='p-2'>
            <DrawerTitle className='text-2xl items-center flex flex-row justify-center'>{props.task.template.name}</DrawerTitle>
            <div className='flex flex-row justify-center' style={{gap: "2px"}}>
              <TaskTypeBadge taskType={props.task.template.taskTypeId}/>
              <Badge className='mx-2 py-2 px-4 rounded-full'>
                [WORKFLOW NAME]
              </Badge>
            </div>
            <Separator/>            
            <DrawerDescription>{props.task.template.description}</DrawerDescription>
          </DrawerHeader>
          {
            props.task.template.taskTypeId.toLowerCase() === "checklist" &&
            <ChecklistTask 
              taskInstanceId={props.task.taskInstanceId} 
              checklistInstance={props.task.instanceData as ChecklistTaskInstance} 
              checklistTemplate={props.task.template.taskTypeData as ChecklistTaskTemplate} 
              fetchTaskInstances={props.fetchTaskInstances}
            />
          }
          {
            props.task.template.taskTypeId.toLowerCase() === "upload-document" &&
            <div className='flex flex-col gap-2 p-2 item-center justify-center'>
              <div className='flex flex-col gap-2 item-center justify-center mx-auto'>
                <Label>Supported document types: {(props.task.template.taskTypeData as FileUploadTaskTemplate).supportedDocumentType}</Label>
              </div>
              <form className='mx-auto flex flex-col gap-2 w-100' onSubmit={(event: any) => {event.preventDefault(); alert("File uploaded!");}}>
                <input type='file' className='bg-gray-100 p-2 rounded-full cursor-pointer' required accept={(props.task.template.taskTypeData as FileUploadTaskTemplate).supportedDocumentType}/>
                <Button type='submit'>Upload Document</Button>
              </form>
            </div>
          }
          {
            props.task.template.taskTypeId.toLowerCase() === "read-document" &&
            <div className='flex flex-col gap-2 p-2'>
              <Button onClick={() => {window.open((props.task?.template.taskTypeData as ReadDocumentTaskTemplate).documentUrl, '_blank')}}>Read Document</Button>
              <div className='flex flex-row gap-2 mx-auto'>
                <Checkbox/>
                <Label>{(props.task.template.taskTypeData as ReadDocumentTaskTemplate).checkBoxLabel}</Label>
              </div>
            </div>
          }
          </>
        }
        <DrawerFooter>
          <Button variant={"outline"}>Flag an issue with this task</Button>
          <Button variant={"outline"}>See discussions about this task</Button>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  )
}
