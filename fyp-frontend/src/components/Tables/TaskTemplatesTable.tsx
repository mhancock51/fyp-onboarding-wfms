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
import UpdateTaskTemplateDialog from '@/app/dialogs/taskTemplateDialogs/UpdateTaskTemplateDialog';
import { Input } from '../ui/input';
import { Button } from '../ui/button';
import { X } from 'lucide-react';

interface Props {
  onRowClick?: (taskTemplate: TaskTemplate) => void;
  selectedTemplate: TaskTemplate | null;
  status?: string;  
}

export default function TaskTemplatesTable(props: Props) {
  const baseTaskTemplates = useSelector((state: RootState) => state.app.taskTemplates);
  const dispatch = useDispatch();

  const [selectedTaskTemplate, setSelectedTaskTemplate] = useState<TaskTemplate | null>(null);
  const [openUpdateDialog, setOpenUpdateDialog] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);
  const [filteredTemplates, setFilteredTemplates] = useState<TaskTemplate[]>([]);

  const [searchTerm, setSearchTerm] = useState<string>("");

  async function fetchTaskTemplates() {
    setLoading(true);
    await Api.taskTemplates.fetchAllTaskTemplates(props.status)
    .then((response) => {
      dispatch(SET_TASK_TEMPLATES(baseTaskTemplates));
      filterTemplates();
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
    if (status === undefined) { return ""}
    switch(status.toLowerCase()) {
      case "active":
        return "bg-primary";
      case "archived":
        return "bg-gray-300";
    }
  }

  function filterTemplates() {
    const filteredTemplates = baseTaskTemplates.filter(t => 
      t.name.toLowerCase().includes(searchTerm.toLowerCase()) || 
      t.taskType.taskName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredTemplates(filteredTemplates);
  }

  useEffect(() => {
    void fetchTaskTemplates();    
  }, []);

  useEffect(() => {
    filterTemplates();
  }, [searchTerm]);

  return (
    <div className='flex flex-col gap-2'>
      <div className='flex flex-row w-full gap-2'>
        <Input disabled={loading} className='flex-11' value={searchTerm} onChange={(event: any) => {setSearchTerm(event.target.value)}}
          type='text' placeholder='Enter search term...' 
        />
        <Button className='flex-1' onClick={() => {setSearchTerm("");}}><X/></Button>  
      </div>
      <div className='h-[55vh] overflow-y-auto flex flex-col gap-2'>
        {
          filteredTemplates.length !== 0 &&
          <Table>
            <TableHeader>
              <TableCell width={200}>Name</TableCell>
              <TableCell className='text-center' width={25}>Task Type</TableCell>
              <TableCell className='text-center' width={25}>Date Created</TableCell>
              <TableCell className='text-center' width={75}>Status</TableCell>
              <TableCell className='text-center' width={75}>Active Instances</TableCell>
              <TableCell width={25}></TableCell>
            </TableHeader>
            <TableBody>
              {
                filteredTemplates.map((taskTemplate, index) => (
                  <TableRow key={index} className={`${props.selectedTemplate?.id === taskTemplate.id ? "bg-secondary" : ""} cursor-pointer hover:bg-accent`} onClick={() => { if (props.onRowClick) props.onRowClick(taskTemplate);}}>
                    <TableCell className={`${props.selectedTemplate?.id === taskTemplate.id ? "font-bold" : ""}`}>{taskTemplate.name}</TableCell>
                    <TableCell className='p-3'>
                      <TaskTypeBadge taskTypeId={taskTemplate.taskTypeId}/>                  
                    </TableCell>
                    <TableCell>{new Date(taskTemplate.dateCreated).toLocaleString()}</TableCell>
                    <TableCell>
                      <Badge className={`p-2 w-full rounded-full ${statusToColour(taskTemplate.status)}`}>{taskTemplate.status?.toUpperCase()}</Badge>
                    </TableCell>
                    <TableCell className='text-center'>
                      {taskTemplate.activeInstances}
                    </TableCell>
                    <TableCell>
                      <TableActionsDropdown actions={[
                        {
                          label: 'Update Template',
                          onClick: () => {setSelectedTaskTemplate(taskTemplate); setOpenUpdateDialog(true);}
                        },                 
                        {
                          label: 'Archive Template',
                          onClick: () => {void archiveTemplate(taskTemplate.id)}
                        },
                      ]}/>
                    </TableCell>
                  </TableRow>
                ))
              }
            </TableBody>
          </Table>
        }
        {
          !loading && baseTaskTemplates.length === 0 &&
          <NoResults text={'No task templates found'}/>
        }
        {
          loading && baseTaskTemplates.length === 0 &&
          <div className='w-full flex flex-row justify-center gap-2'>
            <Spinner/> Loading templates...
          </div>
        }
      </div>
    </div>
  )
}
