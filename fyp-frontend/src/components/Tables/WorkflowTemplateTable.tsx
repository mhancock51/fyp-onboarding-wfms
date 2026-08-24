import { useEffect, useState } from 'react'
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
import TableActionsDropdown from '../TableActionsDropdown';
import { Badge } from '../ui/badge';
import WorkflowTemplateFeedbackDialog from '@/app/dialogs/WorkflowTemplateFeedbackDialog';

interface Props {
  onTemplateSelected: (selectedWorkflowTemplate: WorkflowTemplateDTO) => void;
  selectedTemplate: WorkflowTemplateDTO | null;
}

export default function WorkflowTemplateTable(props: Props) {
  const workflowTemplates = useSelector((state: RootState) => state.app.workflowTemplates);
  const dispatch = useDispatch();

  const [searchTerm, setSearchTerm] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [openFeedbackDialog, setOpenFeedbackDialog] = useState<boolean>(false);

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

  async function archiveWorkflowTemplate(workflowTemplateId: string) {
    setLoading(true);
    await Api.workflowTemplates.archiveWorkflowTemplate(workflowTemplateId)
    .then((response) => {
      toast.success("Successfully archived workflow template");
      void fetchWorkflowTemplates();
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to archive workflow template");
      } 
    })
  }

  function filterTemplates() {
    const filteredTemplates = workflowTemplates.filter(wf => 
      wf.name.toLowerCase().includes(searchTerm.toLowerCase()) || wf.description.toLowerCase().includes(searchTerm.toLowerCase()))
    setFilteredTemplates(filteredTemplates);
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
              <TableCell width={15}>Status</TableCell>
              <TableCell width={10}>Tasks</TableCell>
              <TableCell width={25}></TableCell>
            </TableHeader>
            <TableBody>
              {
                filteredTemplates.map((workflowTemplate, index) => (
                  <TableRow key={index} onClick={() => {props.onTemplateSelected(workflowTemplate)}}
                    className={`${props.selectedTemplate?.id === workflowTemplate.id ? "bg-secondary" : ""} cursor-pointer hover:bg-muted/50`}
                  >
                    <TableCell>{workflowTemplate.name}</TableCell>
                    <TableCell>{workflowTemplate.isOnboardingWF ? "Onboarding Workflow" : "Workflow"}</TableCell>
                    <TableCell>
                      <Badge className={`p-2 w-full rounded-full w-[80px] ${statusToColour(workflowTemplate.status)}`}>
                        {workflowTemplate.status.toUpperCase()}
                      </Badge>
                    </TableCell>
                    <TableCell>{workflowTemplate.numberOfTasks}</TableCell>
                    <TableCell>
                      <TableActionsDropdown actions={
                        workflowTemplate.status !== "archived" ? [
                        {
                          label: 'View Feedback',
                          onClick: () => {props.onTemplateSelected(workflowTemplate); setOpenFeedbackDialog(true);}
                        },
                        {
                          label: 'Archive workflow template',
                          onClick: () => {void archiveWorkflowTemplate(workflowTemplate.id);}
                        },
                      ] : []}/>

                    </TableCell>
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
      {
        props.selectedTemplate !== null &&
        <WorkflowTemplateFeedbackDialog open={openFeedbackDialog} setOpen={setOpenFeedbackDialog} workflowTemplate={props.selectedTemplate}/>
      }
    </div>
  )
}
