import Api from '@/api'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Separator } from '@/components/ui/separator'
import { Spinner } from '@/components/ui/spinner'
import { SET_USER } from '@/features/appSlice'
import HTTPresponse from '@/models/HTTPresponse'
import Utils from '@/util'
import { AxiosResponse } from 'axios'
import { TriangleAlert } from 'lucide-react'
import React, { useState } from 'react'
import { useDispatch } from 'react-redux'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import ChangePasswordForm from './ChangePasswordForm'
import UpdateDetailsForm from './UpdateDetailsForm'

export default function SettingsPage() {
  // const DELETE_ACCOUNT_CONFIRMATION_INPUT = "DELETE MY ACCOUNT";
  const DELETE_ACCOUNT_CONFIRMATION_INPUT = "EEE";

  const [deletingAccount, setDeletingAccount] = useState<boolean>(false);

  const dipatch = useDispatch();
  const navigate = useNavigate();

  function logOutUser() {
    dipatch(SET_USER(null));
    Utils.clearLoginDetailsInLocalStorage();
    navigate("/login");
  }

  async function deleteAccount() {
    setDeletingAccount(true);
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
      setDeletingAccount(false);
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

  return (
    <div className='flex flex-col gap-2 w-full'>
      <h1 className='text-xl text-foreground font-bold m-2'>Settings</h1>
      <Separator/>
      <UpdateDetailsForm/>
      <Separator/>
      <ChangePasswordForm onSuccessful={logOutUser}/>
      {/* Delete account form */}
      <form className='flex flex-col gap-2 w-[50vw] mx-auto' onSubmit={(event: any) => {event.preventDefault(); handleDeleteAccount();}}>
        <h2 className='font-semibold text-md text-foreground'>Delete your account</h2>
        <Label className='font-normal'>This action can't be reversed!</Label>
        <Button variant='destructive' className='w-[300px] mx-auto'>
          {
            deletingAccount &&
            <Spinner className='text-primary-foreground'/>
          }
          <TriangleAlert/> Delete Account
        </Button>
      </form>
    </div>
  )
}
