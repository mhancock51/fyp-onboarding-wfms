import TaskTypeBadge from '@/app/pages/MyTasksPage/TaskTypeBadge'
import React, { useEffect, useState } from 'react'
import { Table, TableHeader, TableCell, TableBody, TableRow } from '../ui/table'
import Api from '@/api';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { toast } from 'sonner';

interface Props {
  onRowClick?: (taskTemplate: TaskTemplate) => void;
  selectedTemplate: TaskTemplate | null;
}

export default function TaskTemplatesTable(props: Props) {
  const [taskTemplates, setTaskTemplates] = useState<TaskTemplate[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

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

  useEffect(() => {
    void fetchTaskTemplates();
  }, []);

  return (
    <Table>
      <TableHeader>
        <TableCell>Name</TableCell>
        <TableCell>Task Type</TableCell>
        <TableCell>Date Created</TableCell>
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
            </TableRow>
          ))
        }
      </TableBody>
    </Table>
  )
}
