import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogTitle } from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Separator } from '@/components/ui/separator';
import { Spinner } from '@/components/ui/spinner';
import { Textarea } from '@/components/ui/textarea';
import IssueDTO from '@/models/DTOs/IssueDTO';
import HTTPresponse from '@/models/HTTPresponse';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'sonner';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  issue: IssueDTO;
  fetchIssues: () => Promise<void>;
}

export default function UpdateIssueStatusDialog(props: Props) {
  const dispatch = useDispatch();

  const [remark, setRemark] = useState<string>("");
  const [status, setStatus] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  async function updateIssue() {
    setLoading(true);
    await Api.issues.updateStatus(status, remark, props.issue.id)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success("Successfully updated task status");
      closeAndClear();
      void props.fetchIssues();
    })
    .catch((error) => {
      toast.error("Failed to update task status");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function closeAndClear() {
    props.setOpen(false);
  }

  useEffect(() => {
    if (props.issue.remark !== undefined) {
      setRemark(props.issue.remark);
    }
    setStatus(props.issue.status);
  }, [props.issue]);

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="min-w-[600px]">
        <DialogTitle>
          Update status of issue       
        </DialogTitle>
        <form className="flex flex-col gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); void updateIssue();}}> 
          <div className='grid grid-cols-4'>
            <Label>Issue Creator</Label>
            <Label className='col-span-3 font-normal'>{props.issue.issueCreatorAccount.displayName} ({new Date(props.issue.issueLoggedTimestamp).toLocaleDateString()})</Label>
          </div>
          <div className='grid grid-cols-4'>
            <Label>Task Template</Label>
            <Label className='col-span-3 font-normal'>{props.issue.taskInstance.template.name}</Label>
          </div>
          <div className='grid grid-cols-4'>
            <Label>Description</Label>
            <Label className='col-span-3 font-normal'>{props.issue.description}</Label>
          </div>
          <div className='grid grid-cols-4'>
            <Label>Suggested changes</Label>
            <Label className='col-span-3 font-normal'>{props.issue.suggestedChanges}</Label>
          </div>
          <Separator/>
          <div className='grid grid-cols-4'>
            <Label>Update Status</Label>
            <Select required value={status} onValueChange={(value: string) => {setStatus(value);}}>
              <SelectTrigger className='col-span-3'>
                <SelectValue placeholder="Select a status"/>
              </SelectTrigger>
              <SelectContent className="w-full z-99">
                <SelectGroup>
                  <SelectItem value='open'>Open</SelectItem>
                  <SelectItem value='resolved'>Resolved</SelectItem>
                  <SelectItem value='closed'>Closed</SelectItem>
                </SelectGroup>
              </SelectContent>              
            </Select>
          </div>
          <div className='grid grid-cols-4'>
            <Label>Remark</Label>
            <Textarea className='col-span-3' placeholder='Leave a comment for the user to see...' value={remark} onChange={(event: any) => {setRemark(event.target.value);}}/>
          </div>
          <DialogFooter>
            <Button type="submit">
              {
                loading &&
                <Spinner className='text-primary-foreground'/>
              }
              Update status
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
