import Api from '@/api';
import TableActionsDropdown from '@/components/TableActionsDropdown';
import { Badge } from '@/components/ui/badge';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table';
import IssueDTO from '@/models/DTOs/IssueDTO'
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { useEffect, useState } from 'react'
import { toast } from 'sonner';
import { useDispatch, useSelector } from 'react-redux';
import UpdateIssueStatusDialog from '@/app/dialogs/UpdateIssueStatusDialog';
import { RootState } from '@/store';
import { Spinner } from '@/components/ui/spinner';
import NoResults from '@/components/NoResults';
import { SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG, SET_SELECTED_TASK_TEMPLATE } from '@/features/appSlice';
import AccountDirectoryBadge from '@/components/AccountDirectoryBadge';

export default function IssuesPage() {  
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.app.user);
  const templates = useSelector((state: RootState) => state.app.taskTemplates);

  const [currentIssue, setCurrentIssue] = useState<IssueDTO | null>(null);
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  const [issues, setIssues] = useState<IssueDTO[]>([]);  
  const [loading, setLoading] = useState<boolean>(false);

  function handleEditTaskTemplateActionClick(issue: IssueDTO) {
    const taskTemplate = templates.find(t => t.id === issue.taskTemplateId);
    if (taskTemplate === undefined) return;    
    dispatch(SET_SELECTED_TASK_TEMPLATE(taskTemplate));
    dispatch(SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG(true));    
  }

  async function fetchAllIssues() {
    setLoading(true);
    Api.issues.fetchAll()
    .then((response: AxiosResponse<HTTPresponse<IssueDTO[], string>>) => {
      setIssues(response.data.data);
    })
    .catch(() => {
      toast.error("Failed to load issues");
    })
    .finally(() => {
      setLoading(false);
    })
  }
  
  async function fetchUsersIssues() {
    setLoading(true);
    Api.issues.fetchUsersIssues()
    .then((response: AxiosResponse<HTTPresponse<IssueDTO[], string>>) => {
      setIssues(response.data.data);
    })
    .catch(() => {
      toast.error("Failed to load issues");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function statusToColor(status: string) {
    switch(status.toLowerCase()) {
      case "open":
        return "bg-blue-500";
      case "closed":
        return "bg-green-500";
      case "resolved":
        return "bg-green-500";      
    }
  }

  useEffect(() => {
    if (user?.isSupervisor) {
      void fetchAllIssues();
    }
    else {
      void fetchUsersIssues();
    }
  }, []);

  return (
    <div>
      <h1 className='text-xl text-foreground font-bold m-2'>Reported Issues {user?.isSupervisor ? "(Showing all issuses)" : ""}</h1>      
      <Separator/>
      <div className='flex flex-col w-full p-2'>
        {
          loading &&
          <div className='flex flex-row justify-center gap-2'>
            <Spinner/>
            Loading issues...
          </div>
        }
        {
          !loading &&
          <Label>Open Issues: {issues.filter(i => i.status === "open").length}</Label>
        }
        {
          issues.length > 0 && !loading &&
          <>
            <Table>
              <TableHeader>
                <TableCell width={50}>Id</TableCell>
                <TableCell className='text-center' width={100}>Task Template</TableCell>
                <TableCell className='text-center' width={75}>Creator</TableCell>              
                <TableCell className='text-center' width={75}>Status</TableCell>
                <TableCell className='text-center' width={75}>Logged</TableCell>
                <TableCell width={1200}>Description</TableCell>
                <TableCell width={1200}>Remark</TableCell>
                <TableCell></TableCell>
              </TableHeader>  
              <TableBody>
                {
                  issues.map((issue, index) => (
                    <TableRow key={index} onClick={() => { setCurrentIssue(issue);}}>
                      <TableCell>{issue.id}</TableCell>
                      <TableCell>
                        <Badge className='p-2 rounded-full min-w-[200px]'>
                          {issue.taskInstance.template.name}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <AccountDirectoryBadge accountDirectory={issue.issueCreatorAccount}/>
                      </TableCell>    
                      <TableCell>
                        <Badge className={`p-2 rounded-full min-w-[100px] ${statusToColor(issue.status)}`}>
                          {issue.status.toUpperCase()}
                        </Badge>
                      </TableCell>
                      <TableCell>{new Date(issue.issueLoggedTimestamp).toLocaleDateString()}</TableCell>
                      <TableCell>
                        {issue.description}
                      </TableCell>      
                      <TableCell>
                        { (issue.remark !== undefined && issue.remark !== "") ? issue.remark : "N/A"}
                      </TableCell>          
                      {
                        user?.isSupervisor &&
                        <TableCell>
                          <TableActionsDropdown actions={[
                            {
                              label: 'Update Status',
                              onClick: () => { setCurrentIssue(issue); setOpenDialog(true);}
                            },
                            {
                              label: 'Edit Task Template',
                              onClick: () => { handleEditTaskTemplateActionClick(issue)}
                            }
                          ]}/>
                        </TableCell>
                      }
                    </TableRow>
                  ))
                }
              </TableBody>
            </Table>          
          </>
        }
        {
          issues.length === 0 && !loading &&
          <NoResults text={'No issues found'}/>
        }
      </div>
      {
        currentIssue !== null &&
        <UpdateIssueStatusDialog open={openDialog} setOpen={setOpenDialog} issue={currentIssue} fetchIssues={fetchAllIssues}/>
      }
    </div>
  )
}
