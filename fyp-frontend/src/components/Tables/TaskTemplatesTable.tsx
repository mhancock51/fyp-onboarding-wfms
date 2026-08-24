import TaskTypeBadge from '@/app/pages/MyTasksPage/TaskTypeBadge'
import { useEffect, useState } from 'react'
import { Table, TableHeader, TableCell, TableBody, TableRow } from '../ui/table'
import Api from '@/api';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { toast } from 'sonner';
import { Spinner } from '../ui/spinner';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';
import { SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG, SET_SELECTED_TASK_TEMPLATE, SET_TASK_TEMPLATES } from '@/features/appSlice';
import NoResults from '../NoResults';
import TableActionsDropdown from '../TableActionsDropdown';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import { Badge } from '../ui/badge';
import ClearableInput from '../ClearableInput';

interface Props {
  onRowClick?: (taskTemplate: TaskTemplate) => void;
  selectedTemplate: TaskTemplate | null;
  status?: string;  
}

export default function TaskTemplatesTable(props: Props) {
  const baseTaskTemplates = useSelector((state: RootState) => state.app.taskTemplates);
  const dispatch = useDispatch();
  const [loading, setLoading] = useState<boolean>(false);
  const [filteredTemplates, setFilteredTemplates] = useState<TaskTemplate[]>([]);

  const [searchTerm, setSearchTerm] = useState<string>("");

  async function fetchTaskTemplates() {
    setLoading(true);
    await Api.taskTemplates.fetchAllTaskTemplates(props.status)
    .then(() => {
      dispatch(SET_TASK_TEMPLATES(baseTaskTemplates));
      filterTemplates();
      setLoading(false);
    })
    .catch(() => {
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

  function handleUpdateTemplateActionClick(taskTemplate: TaskTemplate) {
    dispatch(SET_SELECTED_TASK_TEMPLATE(taskTemplate));
    dispatch(SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG(true));
  }

  useEffect(() => {
    void fetchTaskTemplates();    
  }, []);

  useEffect(() => {
    filterTemplates();
  }, [searchTerm, baseTaskTemplates]);

  return (
    <div className='flex flex-col gap-2 w-auto'>
      <ClearableInput inputType={'text'} value={searchTerm} setValue={setSearchTerm} placeholder='Enter search term...'/>
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
                  <TableRow key={index} className={`${props.selectedTemplate?.id === taskTemplate.id ? "bg-secondary" : ""} cursor-pointer hover:bg-muted/50`} onClick={() => { if (props.onRowClick) props.onRowClick(taskTemplate);}}>
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
                          onClick: () => {handleUpdateTemplateActionClick(taskTemplate);}
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
          !loading && filteredTemplates.length === 0 &&
          <NoResults text={'No task templates found'}/>
        }
        {
          loading && filteredTemplates.length === 0 &&
          <div className='w-full flex flex-row justify-center gap-2'>
            <Spinner/> Loading templates...
          </div>
        }
      </div>
    </div>
  )
}
