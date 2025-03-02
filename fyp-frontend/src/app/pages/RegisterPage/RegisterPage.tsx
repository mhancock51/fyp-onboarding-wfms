import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import InvitedAccount from '@/models/InvitedAccount';
import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';

export default function RegisterPage() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState<boolean>(false);
  const [step, setStep] = useState<number>(0);
  
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [confirmationPassword, setConfirmationPassword] = useState<string>("");
  const [invitedAccount, setInvitedAccount] = useState<InvitedAccount | null>(null);

  async function fetchInvitedAccount() {
    if (step !== 0) return;
    setLoading(true);
    await Api.getInvitedAccount(email)
    .then((response) => {
      setInvitedAccount(response.data.data as InvitedAccount);
      setLoading(false);
      setStep(1);
    })
    .catch((error) => {
      setLoading(false);
      const errorMessage = error.response.data.error;
      if (errorMessage === undefined) {
        toast.error(`Failed to retrieve account`);
      }
      else {
        toast.error(`Failed to retrieve account: ${errorMessage}`);
      }
      
    })
  }

  async function registerAccount() {
    if (step !== 2) return;
    setLoading(true);
    Api.registerAccount(email, password, confirmationPassword)
    .then((response) => {
      console.log(response);
      setLoading(false);
      toast("Successfully registered user", { duration: 1000, onAutoClose: () => { navigate("/login");}});
    })
    .catch((error) => {
      setLoading(false);
      const errorMessage = error.response.data.error;
      if (errorMessage === undefined) {
        toast.error(`Failed to register account`);
      }
      else {
        toast.error(`Failed to register account: ${errorMessage}`);
      }
    })
  }

  return (
    <div className='center-canvas'>
      <div className='m-auto w-96' >
      <Card style={{minHeight: "30vh"}}>
        <CardHeader>
        <CardTitle className="text-2xl">Register</CardTitle>
          <CardDescription className='flex flex-col gap-4'>
            {
              step === 0 &&
              "Register your account" 
            }
            {
              step === 1 &&
              "Confirm your details"
            }
            <span style={{color: "red", textAlign: "center"}}>{loading ? "loading..." : ""}</span>
          </CardDescription>
        </CardHeader>
        <CardContent>
          {
            step === 0 &&
            <>
              <div className="flex flex-col gap-6">
                <div className="grid gap-2">
                  <Label >Email</Label>
                  <Input required type="email" placeholder="m@example.com" value={email} onChange={(event: any) => {setEmail(event.target.value);}}/>
                </div>
                <Button type="submit" disabled={loading} className="w-full flex-6s" onClick={fetchInvitedAccount}>Next</Button>
              </div>
              <div className="mt-4 text-center text-sm">
                Already have an account?{" "}
                <a href="/login" className="underline underline-offset-4">Login</a>
              </div>
            </>
          }
          {
            step === 1 &&
            <div className="flex flex-col gap-6">
              <div className="flex flex-row gap-2">
                <Label className='flex-4'>Display Name</Label>
                <Label className='font-normal'>{invitedAccount?.displayName}</Label>
              </div>
              <div className="flex flex-row gap-2">
                <Label className='flex-4'>Department</Label>
                <Label className='font-normal'>{invitedAccount?.departmentName}</Label>
              </div>
              <div className="flex flex-row gap-2">
                <Label className='flex-4'>Organisation</Label>
                <Label className='font-normal'>{invitedAccount?.organisationName}</Label>
              </div>
              <Button type="submit" disabled={loading} className="w-full flex-6s" onClick={() => {setStep(2)}}>Confirm</Button>
            </div>
          }
          {
            step === 2 &&
            <div className="flex flex-col gap-6">
              <div className="grid gap-2">
                <div className="flex items-center">
                  <Label>Password</Label>
                </div>
                <Input type="password" value={password} required onChange={(event: any) => {setPassword(event.target.value);}}/>
                <div className="flex items-center">
                  <Label>Confirmation Password</Label>
                </div>
                <Input type="password" value={confirmationPassword} required onChange={(event: any) => {setConfirmationPassword(event.target.value);}}/>
              </div>
              <Button type="submit" disabled={loading || confirmationPassword !== password || password === ""} className="w-full flex-6s" onClick={registerAccount}>Register</Button>
            </div>
          }
        </CardContent>
      </Card>
      </div>
    </div>
  )
}
