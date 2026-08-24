import Api from '@/api'
import AuthenticatedUser from '@/models/AuthenticatedUser';
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@radix-ui/react-dropdown-menu'
import { useState } from 'react'
import { useDispatch } from 'react-redux';
import { SET_USER } from '@/features/appSlice';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { Checkbox } from '@/components/ui/checkbox';
import Utils from '@/util';
import { CheckedState } from '@radix-ui/react-checkbox';
import { Spinner } from '@/components/ui/spinner';

export default function LoginPage() {
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [rememberMe, setRememberMe] = useState<boolean>(false);

  const [loading, setLoading] = useState<boolean>(false);

  const dispatch = useDispatch();
  const navigate = useNavigate();

  async function Login() {
    setLoading(true);
    await Api.fetchLogin(email, password)
    .then((response) => {
      console.log("Response from server:", response);
      if (response.status === 200) {
        // successful login
        const authUser: AuthenticatedUser = response.data.data as AuthenticatedUser;
        dispatch(SET_USER(authUser));        
        toast("Successfully logged in", { duration: 600, onAutoClose: () => {navigate("/");}});
        if (rememberMe) {
          // save email and password
          Utils.saveLoginDetailsToLocalStorage(email, password);
        }
      }
    })
    .catch((error) => {
      const errorMessage = error.response.data.error;
      toast.error(errorMessage);      
      setLoading(false);
    });
  }

  return (
    <div className='canvas center-canvas'>
      <div className='m-auto w-96'>
      <Card className='min-h-[500px]'>
        <CardHeader>
        <CardTitle className="text-2xl">Login</CardTitle>
        </CardHeader>
        <CardContent>
          <div>
            <div className="flex flex-col gap-4">
              <div className="grid gap-2">
                <Label>Email</Label>
                <Input type="email" placeholder="m@example.com" value={email} required onChange={(event: any) => {setEmail(event.target.value);}}/>
              </div>
              <div className="grid gap-2">
                <div className="flex items-center">
                  <Label>Password</Label>
                </div>
                <Input type="password" value={password} required onChange={(event: any) => {setPassword(event.target.value);}}/>
              </div>
              <div className="gap-2 flex flex-row items-center">
                <label
                  htmlFor="remember-me"
                  className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 flex-6"
                >Remember me?</label>
                <Checkbox id="remember-me" onCheckedChange={(checkState: CheckedState) => {setRememberMe(checkState as boolean);}}/>
              </div>
              <Button type="submit" disabled={loading} className="w-full flex-6s" onClick={() => {void Login();}}>
                {
                  loading && <Spinner className="text-primary-foreground"/>
                }
                Login
              </Button>
            </div>
          </div>
        </CardContent>
        <CardFooter className='flex flex-row w-full justify-center'>
          <div className="mt-4 text-center text-sm">
            Don&apos;t have an account?{" "}
            <a onClick={() => navigate("/register")} className="underline underline-offset-4 cursor-pointer">Sign up</a>
          </div>
        </CardFooter>
      </Card>
      </div>
    </div>
  )
}
