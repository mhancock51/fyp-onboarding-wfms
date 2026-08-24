import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator';
import { Spinner } from '@/components/ui/spinner';
import HTTPresponse from '@/models/HTTPresponse';
import { AxiosResponse } from 'axios';
import { useState } from 'react'
import { toast } from 'sonner';

interface Props {
  onSuccessful: () => void;
}

export default function ChangePasswordForm(props: Props) {
  const [oldPassword, setOldPassword] = useState<string>("");
  const [newPassword, setNewPassword] = useState<string>("");
  const [confirmationNewPassword, setConfirmationNewPassword] = useState<string>("");

  const [loading, setLoading] = useState<boolean>(false);

  async function updatePassword() {
    if (newPassword !== confirmationNewPassword) {
      toast.error("New passwords don't match");
      return;
    }

    setLoading(true);
    await Api.account.updatedPassword(oldPassword, newPassword)
    .then(() => {
      toast.success("Successfully updated password, logging you out");
      setNewPassword("");
      setConfirmationNewPassword("");
      setOldPassword("");
      props.onSuccessful();
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to updated password");
      } 
    })
    .finally(() => {
      setLoading(false);
    })
  }

  return (
    <>
    <form className='flex flex-col gap-2 w-[50vw] mx-auto' onSubmit={(event: any) => {event.preventDefault(); void updatePassword();}}>
      <h2 className='font-semibold text-md text-foreground'>Change your password</h2>
      <div className='grid grid-cols-4'>
        <Label className='font-normal'>Old Password</Label>
        <Input type='password' className='col-span-3' value={oldPassword} required 
          onChange={(event: any) => setOldPassword(event.target.value)}
        />
      </div>
      <div className='grid grid-cols-4'>
        <Label className='font-normal'>New Password</Label>
        <Input type='password' className='col-span-3' value={newPassword} required
          onChange={(event: any) => setNewPassword(event.target.value)}
        />
      </div>
      <div className='grid grid-cols-4'>
        <Label className='font-normal'>Confirm new Password</Label>
        <Input type='password' className='col-span-3' value={confirmationNewPassword} required
          onChange={(event: any) => setConfirmationNewPassword(event.target.value)}
        />
      </div>
      <div className='flex flex-row justify-end w-full'>
        <Button type="submit" className='w-[150px]'>
          {
            loading &&
            <Spinner className="text-primary-foreground"/>
          }
          Update Password
        </Button>
      </div>
    </form>
    <Separator/>
    </>
  )
}
