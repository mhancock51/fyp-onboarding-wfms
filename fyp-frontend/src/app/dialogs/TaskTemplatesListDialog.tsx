import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Table, TableHead, TableHeader, TableCell, TableBody, TableRow } from '@/components/ui/table';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';
import TaskTypeBadge from '../pages/MyTasksPage/TaskTypeBadge';
import { useDispatch } from 'react-redux';
import { SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG, SET_OPEN_TASK_TEMPLATES_LIST_DIALOG } from '@/features/appSlice';

interface Props {
  open: boolean;
}

export default function TaskTemplatesListDialog(props: Props) {
  const [taskTemplates, setTaskTemplates] = useState<TaskTemplate[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  const dispatcher = useDispatch();

  async function fetchTaskTemplates() {
    setLoading(true);
    await Api.fetchAllTaskTemplates()
    .then((response) => {
      setTaskTemplates(response.data.data);
      setLoading(false);
    })
    .catch((error) => {
      toast.error("Failed to fetch task templates");
      setLoading(false);
    })
  }

  function closeAndClear() {
    dispatcher(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(false));
  }

  function openCreateTemplateMenu() {
    dispatcher(SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG(true));
    dispatcher(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(false));
  }

  useEffect(() => {
    void fetchTaskTemplates();
  }, []);

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[750px]">
        <DialogHeader>
          <DialogTitle>Task Templates</DialogTitle>          
        </DialogHeader>
        <Table>
          <TableHeader>
            <TableCell>Name</TableCell>
            <TableCell>Task Type</TableCell>
            <TableCell>Date Created</TableCell>
          </TableHeader>
          <TableBody>
            {
              taskTemplates.map((row, index) => (
                <TableRow key={index}>
                  <TableCell>{row.name}</TableCell>
                  <TableCell className='p-3'>
                    <TaskTypeBadge taskTypeId={row.taskTypeId}/>                  
                  </TableCell>
                  <TableCell>{new Date(row.dateCreated).toLocaleString()}</TableCell>
                </TableRow>
              ))
            }
          </TableBody>
        </Table>
        <DialogFooter>
          <Button type='button' onClick={openCreateTemplateMenu}>Create Template</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
