import Api from '@/api';
import AccountDirectoryLookup from '@/components/AccountDirectoryLookup';
import DepartmentLookup from '@/components/DepartmentLookup';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import WorkflowTemplateLookup from '@/components/WorkflowTemplateLookup';
import { SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG } from '@/features/appSlice';
import AccountDirectory from '@/models/AccountDirectory';
import Department from '@/models/Department';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import HTTPresponse from '@/models/HTTPresponse';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import React, { useState } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'sonner';

export default function CreateWorkflowInstanceDialog() {
  const [workflowTemplate, setWorkflowTemplate] = useState<WorkflowTemplateDTO | null>(null);
  const [supervisor, setSupervisor] = useState<AccountDirectory | null>(null);

  // Onboarding employee related values
  const [displayName, setDisplayName] = useState<string>("");
  const [emailAddress, setEmailAddress] = useState<string>("");
  const [department, setDepartment] = useState<Department | null>(null);

  const [step, setStep] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);

  const open = useSelector((state: RootState) => state.app.openCreateWorkflowInstanceDialog);
  const dispatch = useDispatch();

  async function createWorkflowInstance() {
    if (workflowTemplate === null) {
      toast.warning("Please select a workflow template");
      return;
    }
    if (supervisor === null) {
      toast.warning("Please select a supervisor");
      return;
    }
    if (workflowTemplate.isOnboardingWF) {
      if (displayName === "") {
        toast.warning("Please select a display name");
        return;
      }
      if (emailAddress === "") {
        toast.warning("Please select an email address");
        return;
      }
      if (department === null) {
        toast.warning("Please select a department");
        return;
      }
    }
    setLoading(true);
    await Api.createWorkflowInstance(workflowTemplate.id, supervisor.id, 
      workflowTemplate.isOnboardingWF ? {
        displayName: displayName, emailAddress: emailAddress, departmentId: department?.id ?? ""
      } : null
    )
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success("Successfully created workflow instance");
      closeAndClear();
    })
    .catch((error) => {      
      toast.error(error.response.data?.error); 
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function closeAndClear() {
    setWorkflowTemplate(null);
    setSupervisor(null);
    setDisplayName("");
    setEmailAddress("");
    setDepartment(null);
    setStep(0);

    dispatch(SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG(false));    
  }

  function nextStep() {
    if (step === 0) {
      if (workflowTemplate === null) {
        toast.warning("Please select a workflow template");
        return;
      }
      if (supervisor === null) {
        toast.warning("Please select a supervisor template");
        return;
      }
      if (workflowTemplate?.isOnboardingWF) {
        setStep(1);
      }
      else {
        setStep(2);
      }
    }
    else if (step === 1) {
      if (department === null) {
        toast.warning("Please select a department");
        return;
      }
      setStep(2);
    }
  }

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="w-[600px]">
        {
          step === 0 &&
          <>
            <DialogHeader>
              Start a workflow instance
            </DialogHeader>
            <form className="flex flex-col gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); nextStep();}}>
              <div className="flex flex-row items-center gap-4">
                <Label htmlFor="name" className="flex-4">Workflow Template</Label>
                <WorkflowTemplateLookup value={workflowTemplate} setValue={setWorkflowTemplate}/>
              </div>
              <div className="flex flex-row items-center gap-4">
                <Label htmlFor="name" className="flex-4">Supervisor</Label>                
                <AccountDirectoryLookup setAccounts={setSupervisor} accounts={supervisor}
                  additionalAccounts={[]} filter={(a: AccountDirectory) => (a.isSupervisor)}
                />
              </div>
              <DialogFooter>              
                <Button type='submit'>Next</Button>
              </DialogFooter>
            </form>
          </>
        }
        {
          step === 1 && workflowTemplate?.isOnboardingWF &&
          <>
            <DialogHeader>
              Enter Onboarding Employee's Details
            </DialogHeader>
            <form className="flex flex-col gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); nextStep();}}>
              <div className="flex flex-row items-center gap-4 w-full">
                <Label htmlFor="name" className="flex-4">Display Name</Label>
                <Input required className='flex-8' value={displayName} onChange={(event: any) => {setDisplayName(event.target.value);}}/>                
              </div>
              <div className="flex flex-row items-center gap-4 w-full">
                <Label className="flex-4">Email Address</Label>
                <Input type='email' required className='flex-8' value={emailAddress} onChange={(event: any) => {setEmailAddress(event.target.value);}}/>                
              </div>
              <div className="flex flex-row items-center gap-4 w-full">
                <Label htmlFor="name" className="flex-4">Department</Label>
                <DepartmentLookup setDepartment={setDepartment} department={department}/>
              </div>
              <DialogFooter>
                <Button type='button' onClick={() => {setStep(0)}}>Back</Button>              
                <Button type='submit'>Next</Button>
              </DialogFooter>
            </form>
          </>
        }
        {
          step === 2 &&
          <>
            <DialogHeader>
              Confirm Details
            </DialogHeader>
            <form className="flex flex-col gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); void createWorkflowInstance()}}>
              <div className="flex flex-row items-center gap-4 w-full">
                <Label className='flex-4'>Workflow Template</Label>
                <Label className='font-normal flex-8'>{workflowTemplate?.name}</Label>                
              </div>
              <div className="flex flex-row items-center gap-4 w-full">
                <Label className='flex-4'>Supervisor</Label>
                <Label className='font-normal flex-8'>{supervisor?.displayName}</Label>                
              </div>
              <div className="flex flex-row items-center gap-4 w-full">
                <Label className='flex-4'>Onboarding Workflow?</Label>
                <Label className='font-normal flex-8'>{workflowTemplate?.isOnboardingWF ? "yes" : "no"}</Label>                
              </div>
              {
                workflowTemplate?.isOnboardingWF && 
                <>
                  <div className="flex flex-row items-center gap-4 w-full">
                    <Label className='flex-4'>Onboarding Employee</Label>
                    <Label className='font-normal flex-8'>{displayName}</Label>                
                  </div>
                  <div className="flex flex-row items-center gap-4 w-full">
                    <Label className='flex-4'>Email Address</Label>
                    <Label className='font-normal flex-8'>{emailAddress}</Label>                
                  </div>
                  <div className="flex flex-row items-center gap-4 w-full">
                    <Label className='flex-4'>Department</Label>
                    <Label className='font-normal flex-8'>{department?.displayName}</Label>                
                  </div>
                </>
              }
              <DialogFooter>          
                <Button type='button' onClick={() => {setStep(1)}}>Back</Button>     
                <Button type='submit'>
                  {
                    loading && <Spinner/>
                  }
                  Start Workflow Instance
                </Button>
              </DialogFooter>
            </form>
          </>
        }
      </DialogContent>
    </Dialog>
  )
}
