import TaskTypeBadge from '@/app/pages/MyTasksPage/TaskTypeBadge'
import React, { useEffect, useState } from 'react'
import { Table, TableHeader, TableCell, TableBody, TableRow } from '../ui/table'
import Api from '@/api';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { toast } from 'sonner';
import { Spinner } from '../ui/spinner';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';
import { SET_TASK_TEMPLATES } from '@/features/appSlice';
import NoResults from '../NoResults';
import TableActionsDropdown from '../TableActionsDropdown';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import { Badge } from '../ui/badge';

interface Props {
  onRowClick?: (taskTemplate: TaskTemplate) => void;
  selectedTemplate: TaskTemplate | null;
  status?: string;
}

export default function TaskTemplatesTable(props: Props) {
  const taskTemplates = useSelector((state: RootState) => state.app.taskTemplates);
  const dispatch = useDispatch();

  const [loading, setLoading] = useState<boolean>(false);

  async function fetchTaskTemplates() {
    setLoading(true);
    await Api.taskTemplates.fetchAllTaskTemplates(props.status)
    .then((response) => {
      dispatch(SET_TASK_TEMPLATES(taskTemplates));
      setLoading(false);
    })
    .catch((error) => {
      toast.error("Failed to fetch task templates");
      setLoading(false);
    })
  }

  async function archiveTemplate(templateId: string) {
    await Api.taskTemplates.archiveTemplate(templateId)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      void fetchTaskTemplates();
      toast.success("Successfully archived template");
    })
    .catch((error) => {
      toast.error("Failed to archive template");
    })

  }

  function statusToColour(status: string) {
    switch(status.toLowerCase()) {
      case "active":
        return "bg-primary";
      case "archived":
        return "bg-gray-300";
    }
  }

  useEffect(() => {
    void fetchTaskTemplates();    
  }, [taskTemplates]);

  return (
    <div className='min-h-[50vh] overflow-y-auto'>
      {
        taskTemplates.length !== 0 &&
        <Table>
          <TableHeader>
            <TableCell width={200}>Name</TableCell>
            <TableCell className='text-center' width={25}>Task Type</TableCell>
            <TableCell className='text-center' width={25}>Date Created</TableCell>
            <TableCell className='text-center' width={75}>Status</TableCell>
            <TableCell width={25}></TableCell>
          </TableHeader>
          <TableBody>
            {
              taskTemplates.map((row, index) => (
                <TableRow key={index} className={`${props.selectedTemplate?.id === row.id ? "bg-secondary" : ""} cursor-pointer hover:bg-accent`} onClick={() => { if (props.onRowClick) props.onRowClick(row);}}>
                  <TableCell className={`${props.selectedTemplate?.id === row.id ? "font-bold" : ""}`}>{row.name}</TableCell>
                  <TableCell className='p-3'>
                    <TaskTypeBadge taskTypeId={row.taskTypeId}/>                  
                  </TableCell>
                  <TableCell>{new Date(row.dateCreated).toLocaleString()}</TableCell>
                  <TableCell>
                    <Badge className={`p-2 w-full rounded-full ${statusToColour(row.status)}`}>{row.status.toUpperCase()}</Badge>
                  </TableCell>
                  <TableCell>
                    <TableActionsDropdown actions={[
                      {
                        label: 'Archive Template',
                        onClick: () => {void archiveTemplate(row.id)}
                      }                      
                    ]}/>
                  </TableCell>
                </TableRow>
              ))
            }
          </TableBody>
        </Table>
      }
      {
        !loading && taskTemplates.length === 0 &&
        <NoResults text={'No task templates found'}/>
      }
      {
        loading && taskTemplates.length === 0 &&
        <div className='w-full flex flex-row justify-center gap-2'>
          <Spinner/> Loading templates...
        </div>
      }
    </div>
  )
}
