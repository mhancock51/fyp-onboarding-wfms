import React, { useEffect, useState } from 'react'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '../ui/table';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import { SET_WORKFLOW_TEMPLATES } from '@/features/appSlice';
import { toast } from 'sonner';
import NoResults from '../NoResults';
import { Spinner } from '../ui/spinner';
import ClearableInput from '../ClearableInput';

interface Props {
  onTemplateSelected: (selectedWorkflowTemplate: WorkflowTemplateDTO) => void;
  selectedTemplateId: string | null;
}

export default function WorkflowTemplateTable(props: Props) {
  const workflowTemplates = useSelector((state: RootState) => state.app.workflowTemplates);
  const dispatch = useDispatch();

  const [searchTerm, setSearchTerm] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  const [filteredTemplates, setFilteredTemplates] = useState<WorkflowTemplateDTO[]>([]);


  async function fetchWorkflowTemplates() {
    setLoading(true);
    await Api.workflowTemplates.fetchAllWorkflowTemplates()
    .then((response: AxiosResponse<HTTPresponse<WorkflowTemplateDTO[], string>>) => {
      dispatch(SET_WORKFLOW_TEMPLATES(response.data.data as WorkflowTemplateDTO[]));
    })
    .catch((error) => {
      toast.error("Failed to load workflow templates");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function filterTemplates() {
    const filteredTemplates = workflowTemplates.filter(wf => 
      wf.name.toLowerCase().includes(searchTerm.toLowerCase()) || wf.description.toLowerCase().includes(searchTerm.toLowerCase()))
    setFilteredTemplates(filteredTemplates);
  }

  useEffect(() => {
    filterTemplates();
  }, [searchTerm, workflowTemplates]);

  useEffect(() => {
    void fetchWorkflowTemplates();
  }, []);

  return (
    <div className='flex flex-col gap-2 w-auto'>
      <ClearableInput inputType={'text'} value={searchTerm} setValue={setSearchTerm} className='w-full' placeholder='Enter search term...' />
      <div className='h-[55vh] overflow-y-auto flex flex-col gap-2'>
        {
          filteredTemplates.length !== 0 &&
          <Table>
            <TableHeader>
              <TableCell width={200}>Name</TableCell>              
              <TableCell width={10}>Type</TableCell>
              <TableCell width={10}>Number of Tasks</TableCell>
              <TableCell>Active Instances</TableCell>
              <TableCell width={25}></TableCell>
            </TableHeader>
            <TableBody>
              {
                filteredTemplates.map((workflowTemplate, index) => (
                  <TableRow key={index} onClick={() => {props.onTemplateSelected(workflowTemplate)}}
                    className={`${props.selectedTemplateId === workflowTemplate.id ? "bg-secondary" : ""} cursor-pointer hover:bg-accent`}
                  >
                    <TableCell>{workflowTemplate.name}</TableCell>
                    <TableCell>{workflowTemplate.isOnboardingWF ? "Onboarding Workflow" : "Workflow"}</TableCell>
                    <TableCell className='text-center'>{workflowTemplate.numberOfTasks}</TableCell>
                    <TableCell></TableCell>
                    <TableCell></TableCell>
                  </TableRow>
                ))
              }
            </TableBody>
          </Table>
        }
        {
          !loading && filteredTemplates.length === 0 &&
          <NoResults text={'Loading workflow templates...'}/>
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
