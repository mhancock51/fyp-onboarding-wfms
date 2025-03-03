import { Drawer, DrawerClose, DrawerContent, DrawerDescription, DrawerFooter, DrawerHeader, DrawerTitle, DrawerTrigger } from '@/components/ui/drawer'
import ListedTaskInstance from '@/models/ListedTaskInstance'
import TaskTypeBadge from './TaskTypeBadge';
import { Separator } from '@/components/ui/separator';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  task: ListedTaskInstance | null;
}

export default function TaskDrawer(props: Props) {
  return (
    <Drawer direction='right' onClose={() => {props.setOpen(false);}} open={props.open}>
      <DrawerContent className="max-w-[600px] w-full p-2"> {/* Override max width */}
        {
          props.task !== null &&
          <>
          <DrawerHeader className='p-2'>
            <DrawerTitle className='text-2xl items-center flex flex-row justify-center'>{props.task.taskName}</DrawerTitle>
            <div className='flex flex-row justify-center' style={{gap: "2px"}}>
              <TaskTypeBadge taskType={props.task.taskType}/>
              <Badge className='mx-2 py-2 px-4 rounded-full'>
                {props.task.workflowName}
              </Badge>
            </div>
            <Separator/>            
            <DrawerDescription>{props.task.description}</DrawerDescription>
          </DrawerHeader>
          {
            props.task.taskType.toLowerCase() === "checklist" &&
            <div className='flex flex-col gap-2 p-2'>
              <div className='flex flex-row gap-2 p-4 rounded-full border-1 border-black items-center'>
                <Checkbox/>
                <Label className='font-normal'>Task 1</Label>
              </div>
              <div className='flex flex-row gap-2 p-4 rounded-full border-1 border-black items-center'>
                <Checkbox/>
                <Label className='font-normal'>Task 2</Label>
              </div>
              <div className='flex flex-row gap-2 p-4 rounded-full border-1 border-black items-center'>
                <Checkbox/>
                <Label className='font-normal'>Task 3</Label>
              </div>
            </div>
          }
          {
            props.task.taskType.toLowerCase() === "document upload" &&
            <div className='flex flex-col gap-2 p-2 item-center justify-center'>
              <div className='flex flex-col gap-2 item-center justify-center mx-auto'>
                <Label>Supported document types: .pdf</Label>
              </div>
              <form className='mx-auto flex flex-col gap-2 w-100' onSubmit={(event: any) => {event.preventDefault(); alert("File uploaded!");}}>
                <input type='file' className='bg-gray-100 p-2 rounded-full cursor-pointer' required accept='.pdf'/>
                <Button type='submit'>Upload Document</Button>
              </form>
            </div>
          }
          {
            props.task.taskType.toLowerCase() === "read document" &&
            <div className='flex flex-col gap-2 p-2'>
              <Button>Read Document</Button>
              <div className='flex flex-row gap-2 mx-auto'>
                <Checkbox/>
                <Label>I have read and agreed to terms</Label>
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
