import { Drawer, DrawerClose, DrawerContent, DrawerDescription, DrawerFooter, DrawerHeader, DrawerTitle, DrawerTrigger } from '@/components/ui/drawer'
import ListedTaskInstance from '@/models/ListedTaskInstance'
import TaskTypeBadge from './TaskTypeBadge';
import { Separator } from '@/components/ui/separator';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  task: ListedTaskInstance | null;
}

export default function TaskDrawer(props: Props) {
  return (
    <Drawer direction='right' onClose={() => {props.setOpen(false);}} open={props.open}>
      <DrawerContent className="max-w-[600px] w-full"> {/* Override max width */}
        {
          props.task !== null &&
          <DrawerHeader className="max-w-[600px] w-full">
            <div className='flex flex-col gap-2 justify-center'>
              <DrawerTitle className='text-2xl items-center flex flex-row justify-center'>{props.task.taskName}</DrawerTitle>
              <TaskTypeBadge taskType={props.task.taskType}/>
            </div>
            <Separator/>
            <div className='flex flex-row gap-2 p-2 justify-center'>
            </div>
            <DrawerDescription>{props.task.description}</DrawerDescription>
          </DrawerHeader>
        }
      </DrawerContent>
    </Drawer>
  )
}
