import Api from '@/api'
import AuthenticatedUser from '@/models/AuthenticatedUser';
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@radix-ui/react-dropdown-menu'
import { useEffect, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux';
import { SET_USER } from '@/features/appSlice';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { RootState } from '@/store';

export default function LoginPage() {
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  const [loading, setLoading] = useState<boolean>(false);

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);

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
      }
    })
    .catch((error) => {
      const errorMessage = error.response.data.error;
      toast.error(errorMessage);      
      setLoading(false);
    });
  }

  useEffect(() => {
    if (user !== null) {      
      navigate("/");
    }
  }, []);

  return (
    <div style={{width: "100vw"}}>
      <h1 className='text-3xl m-3' style={{textAlign: "center"}}>BoardFlow</h1>
      <div className='m-auto w-96'>
      <Card>
        <CardHeader>
        <CardTitle className="text-2xl">Login</CardTitle>
        <CardDescription className='flex flex-col gap-4'>
          Enter your email below to login to your account
          <span style={{color: "red", textAlign: "center"}}>{loading ? "loading..." : ""}</span>
        </CardDescription>
        </CardHeader>
        <CardContent>
        <div>
          <div className="flex flex-col gap-6">
            <div className="grid gap-2">
              <Label>Email</Label>
              <Input type="email" placeholder="m@example.com" value={email} required onChange={(event: any) => {setEmail(event.target.value);}}/>
            </div>
            <div className="grid gap-2">
              <div className="flex items-center">
                <Label>Password</Label>
                <a href="#" className="ml-auto inline-block text-sm underline-offset-4 hover:underline">
                  Forgot your password?
                </a>
              </div>
              <Input type="password" value={password} required onChange={(event: any) => {setPassword(event.target.value);}}/>
            </div>
            <Button type="submit" disabled={loading} className="w-full" onClick={() => {void Login();}}>Login</Button>
          </div>
          <div className="mt-4 text-center text-sm">
            Don&apos;t have an account?{" "}
            <a href="#" className="underline underline-offset-4">Sign up</a>
          </div>
        </div>
        </CardContent>
      </Card>
      </div>
    </div>
  )
}
