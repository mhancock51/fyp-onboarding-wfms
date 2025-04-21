import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import { SET_OPEN_REPORT_ISSUE_DIALOG } from '@/features/appSlice';
import HTTPresponse from '@/models/HTTPresponse';
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import React, { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'sonner';

interface Props {
  taskInstance: TaskInstanceDTO;
}

export default function ReportIssueDialog(props: Props) {
  const open = useSelector((state: RootState) => state.app.openReportIssueDialog);
  const dispatch = useDispatch();

  const [issueDescription, setIssueDescription] = useState<string>("");
  const [suggestedChanges, setSuggestedChanges] = useState<string>("");
  const [creating, setCreating] = useState<boolean>(false);


  function closeAndClear() {
    dispatch(SET_OPEN_REPORT_ISSUE_DIALOG(false));
    setIssueDescription("");
    setSuggestedChanges("");
  }

  async function createIssue() {
    setCreating(true);
    await Api.issues.createIssue(props.taskInstance.id, issueDescription, suggestedChanges)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success(`Successfully created issue for task ${props.taskInstance.template.name}`);
      closeAndClear();
    })
    .catch((error) => {
      console.log(error);
      toast.error("Failed to create issue for task");
    })
    .finally(() => {
      setCreating(false);
    })
  }

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Report Issue with Task ({props.taskInstance.template.name})</DialogTitle>           
        </DialogHeader>                  
          <form className="flex flex-col py-4 w-full gap-2" onSubmit={(event: any) => {event.preventDefault(); void createIssue();}}>
            <div className="flex flex-col gap-1">
              <Label>Describe The Issue</Label>
              <Textarea required value={issueDescription} onChange={(event: any) => {setIssueDescription(event.target.value)}}/>
            </div>
            <div className="flex flex-col gap-1">
              <Label>Suggest Changes</Label>
              <Textarea required value={suggestedChanges} onChange={(event: any) => {setSuggestedChanges(event.target.value)}}/>
            </div>
            <DialogFooter>
              <Button type='submit'>
                {
                  creating &&
                  <Spinner className="text-primary-foreground"/>
                }
                Submit Issue
              </Button>
            </DialogFooter>
          </form>
      </DialogContent>      
    </Dialog>
  )
}
