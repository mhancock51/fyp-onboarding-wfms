import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { useDispatch } from 'react-redux';
import { SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG, SET_OPEN_TASK_TEMPLATES_LIST_DIALOG } from '@/features/appSlice';
import TaskTemplatesTable from '@/components/Tables/TaskTemplatesTable';

interface Props {
  open: boolean;
}

export default function TaskTemplatesListDialog(props: Props) {  
  const dispatcher = useDispatch();  

  function closeAndClear() {
    dispatcher(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(false));
  }

  function openCreateTemplateMenu() {
    dispatcher(SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG(true));
    dispatcher(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(false));
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="min-w-[900px]">
        <DialogHeader>
          <DialogTitle>Task Templates</DialogTitle>          
        </DialogHeader>
        <TaskTemplatesTable selectedTemplate={null}/>
        <DialogFooter>
          <Button type='button' onClick={openCreateTemplateMenu}>Create Template</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
