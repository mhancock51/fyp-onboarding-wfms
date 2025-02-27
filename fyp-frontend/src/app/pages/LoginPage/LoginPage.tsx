import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@radix-ui/react-dropdown-menu'

export default function LoginPage() {
  return (
    <div style={{width: "100vw"}}>
      <h1 className='text-3xl m-3' style={{textAlign: "center"}}>BoardFlow</h1>
      <div className='m-auto w-96'>
      <Card>
        <CardHeader>
        <CardTitle className="text-2xl">Login</CardTitle>
        <CardDescription>
          Enter your email below to login to your account
        </CardDescription>
        </CardHeader>
        <CardContent>
        <form>
          <div className="flex flex-col gap-6">
            <div className="grid gap-2">
              <Label>Email</Label>
              <Input
              id="email"
              type="email"
              placeholder="m@example.com"
              required
              />
            </div>
            <div className="grid gap-2">
              <div className="flex items-center">
              <Label>Password</Label>
              <a
                  href="#"
                  className="ml-auto inline-block text-sm underline-offset-4 hover:underline"
              >
                  Forgot your password?
              </a>
              </div>
              <Input id="password" type="password" required />
            </div>
            <Button type="submit" className="w-full">Login</Button>
          </div>
          <div className="mt-4 text-center text-sm">
            Don&apos;t have an account?{" "}
            <a href="#" className="underline underline-offset-4">
                Sign up
            </a>
          </div>
        </form>
        </CardContent>
      </Card>
      </div>
    </div>
  )
}
