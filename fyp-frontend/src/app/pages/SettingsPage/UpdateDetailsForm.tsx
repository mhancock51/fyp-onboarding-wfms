import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import HTTPresponse from '@/models/HTTPresponse';
import { RootState } from '@/store';
import Utils from '@/util';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { useSelector } from 'react-redux';
import { toast } from 'sonner';

export default function UpdateDetailsForm() {
  const user = useSelector((state: RootState) => state.app.user);

  const [displayName, setDisplayName] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  async function updateAccount() {
    setLoading(true);
    await Api.account.updateAccount(displayName, email)
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
      setDisplayName(user.displayName);
      setEmail(user.emailAddress);
    }
  }, [user]);

  return (
    <form className='flex flex-col gap-2 w-[50vw] mx-auto' onSubmit={(event: any) => {event.preventDefault(); void updateAccount();}}>
      <h2 className='font-semibold text-md text-foreground'>Update your details</h2>
      <div className='grid grid-cols-4'>
        <Label className='font-normal'>Display Name</Label>
        <Input type='text' className='col-span-3' value={displayName} onChange={(event: any) => setDisplayName(event.target.value)} required/>
      </div>
      <div className='grid grid-cols-4'>
        <Label className='font-normal'>Email Address</Label>
        <Input type='email' className='col-span-3' value={email} onChange={(event: any) => setEmail(event.target.value)} required/>
      </div>
      <div className='grid grid-cols-4'>
        <Label className='font-normal'>Department</Label>
        <Input type='text' className='col-span-3' readOnly disabled value={user?.departmentName}/>
      </div>
      <div className='flex flex-row justify-end w-full'>
        <Button type="submit" className='w-[150px]'>
          {
            loading &&
            <Spinner className='text-primary-foreground'/>
          }
          Update Details
        </Button>
      </div>
    </form>
  )
}
