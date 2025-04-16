import Api from '@/api'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Separator } from '@/components/ui/separator'
import { Spinner } from '@/components/ui/spinner'
import { SET_USER } from '@/features/appSlice'
import HTTPresponse from '@/models/HTTPresponse'
import { RootState } from '@/store'
import Utils from '@/util'
import { AxiosResponse } from 'axios'
import { TriangleAlert } from 'lucide-react'
import React, { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import ChangePasswordForm from './ChangePasswordForm'

export default function SettingsPage() {
  // const DELETE_ACCOUNT_CONFIRMATION_INPUT = "DELETE MY ACCOUNT";
  const DELETE_ACCOUNT_CONFIRMATION_INPUT = "EEE";

  const user = useSelector((state: RootState) => state.app.user);
  const dipatch = useDispatch();
  const navigate = useNavigate();

  const [displayName, setDisplayName] = useState<string>("");
  
  const [loading, setLoading] = useState<boolean>(false);

  function logOutUser() {
    dipatch(SET_USER(null));
    Utils.clearLoginDetailsInLocalStorage();
    navigate("/login");
  }

  async function deleteAccount() {
    setLoading(true);
    await Api.account.deleteAccount()
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      logOutUser();
      toast.success("Successfully delete account");
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to delete account");
      } 
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function handleDeleteAccount() {
    const confirmationInput = prompt(`Enter '${DELETE_ACCOUNT_CONFIRMATION_INPUT}' to confirm you want to delete your account`);
    if (confirmationInput === null || confirmationInput === "") return;
    if (confirmationInput !== DELETE_ACCOUNT_CONFIRMATION_INPUT) {
      toast.warning("Incorrect confirmation input given, try again if you still want to delete you account");
      return;
    }
    // send request to delete account
    void deleteAccount();
  }

  async function updateAccount() {
    setLoading(true);
    await Api.account.updateAccount(displayName)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      Utils.relogin();
      toast.success("Successfully updated account");
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to updated account");
      } 
    })
    .finally(() => {
      setLoading(false);
    })
  }

  useEffect(() => {
    if (user !== null) {
      setDisplayName(user?.displayName);
    }
  }, [user]);

  return (
    <div className='flex flex-col gap-2 w-full'>
      <h1 className='text-xl text-foreground font-bold m-2'>Settings</h1>
      <Separator/>
      {/* Update account form */}
      <form className='flex flex-col gap-2 w-[50vw] mx-auto' onSubmit={(event: any) => {event.preventDefault(); void updateAccount();}}>
        <h2 className='font-semibold text-md text-foreground'>Update your details</h2>
        <div className='grid grid-cols-4'>
          <Label className='font-normal'>Email Address</Label>
          <Input type='text' className='col-span-3' readOnly disabled value={user?.emailAddress}/>
        </div>
        <div className='grid grid-cols-4'>
          <Label className='font-normal'>Department</Label>
          <Input type='text' className='col-span-3' readOnly disabled value={user?.departmentName}/>
        </div>
        <div className='grid grid-cols-4'>
          <Label className='font-normal'>Display Name</Label>
          <Input type='text' className='col-span-3' value={displayName} onChange={(event: any) => setDisplayName(event.target.value)} required/>
        </div>
        <div className='flex flex-row justify-end w-full'>
          <Button type="submit">
            {
              loading &&
              <Spinner className='text-primary-foreground'/>
            }
            Update Details
          </Button>
        </div>
      </form>
      <Separator/>
      <ChangePasswordForm/>
      {/* Delete account form */}
      <form className='flex flex-col gap-2 w-[50vw] mx-auto' onSubmit={(event: any) => {event.preventDefault(); handleDeleteAccount();}}>
        <h2 className='font-semibold text-md text-foreground'>Delete your account</h2>
        <Label className='font-normal'>This action can't be reversed!</Label>
        <Button variant='destructive' className='w-[300px] mx-auto'>
          {
            loading &&
            <Spinner className='text-primary-foreground'/>
          }
          <TriangleAlert/> Delete Account
        </Button>
      </form>
    </div>
  )
}
