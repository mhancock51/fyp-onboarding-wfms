import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { SET_OPEN_REPORT_ISSUE_DIALOG } from '@/features/appSlice';
import { RootState } from '@/store';
import React from 'react'
import { useDispatch, useSelector } from 'react-redux';

export default function ReportIssueDialog() {
  const open = useSelector((state: RootState) => state.app.openReportIssueDialog);

  const dispatch = useDispatch();

  function closeAndClear() {
    dispatch(SET_OPEN_REPORT_ISSUE_DIALOG(false));
  }

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Report Issue with Task</DialogTitle>           
        </DialogHeader>                  
          <form className="flex flex-col py-4 w-full gap-2">
            <div className="flex flex-col gap-1">
              <Label>Describe The Issue</Label>
              <Textarea required/>
            </div>
            <div className="flex flex-col gap-1">
              <Label>Suggest Changes</Label>
              <Textarea required/>
            </div>
            <div className="flex flex-col gap-1">
              <Label>Upload screenshots</Label>
              <Input type='file'/>
            </div>
            <DialogFooter>
              <Button type='submit'>Submit Issue</Button>
            </DialogFooter>
          </form>
      </DialogContent>      
    </Dialog>
  )
}
