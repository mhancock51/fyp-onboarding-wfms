import Api from "@/api";
import DepartmentLookup from "@/components/DepartmentLookup";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Spinner } from "@/components/ui/spinner";
import Department from "@/models/Department";
import { useState } from "react";
import { toast } from "sonner";

export default function InviteUserDialog(props: {open: boolean, setOpenDialog: (open: boolean) => void}) {
  const [onboarder, setOnboarder] = useState<boolean>(false);

  const [department, setDepartment] = useState<Department | null>(null);
  const [displayName, setDisplayName] = useState<string>("");
  const [email, setEmail] = useState<string>("");

  const [loading, setLoading] = useState<boolean>(false);

  async function inviteUser() {
    if (displayName === "") {
      toast.warning("Please enter display name");
      return;
    }
    if (email === "") {
      toast.warning("Please enter an email address");
      return;
    }
    if (department === null) {
      toast.warning("Please select a department");
      return;
    }

    setLoading(true);
    Api.inviteUser(displayName, email, onboarder, department.id)
    .then((response) => {
      setLoading(false);
      toast(`Successfully invited ${displayName}!`, { duration: 600, onAutoClose: () => {
        closeAndClear();
      }})
    })
    .catch((error) => {
      setLoading(false);
      const errorMessage = error.response.data.error;
      if (errorMessage === undefined) {
        toast.error(`Failed to invite user`);
      }
      else {
        toast.error(`Failed to invite user: ${errorMessage}`);
      }
    })
  }

  function closeAndClear() {
    setOnboarder(false);
    setDepartment(null);
    setDisplayName("");
    setEmail("");
    props.setOpenDialog(false);
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Invite user</DialogTitle>
          <DialogDescription>
            Invite an employee to the organisation            
          </DialogDescription>
        </DialogHeader>
        <form className="grid gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); inviteUser();}}>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Display Name</Label>
            <Input required className="col-span-3" value={displayName} onChange={(event: any) => {setDisplayName(event.target.value);}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Email Address</Label>
            <Input required type="email" className="col-span-3" value={email} onChange={(event: any) => {setEmail(event.target.value);}} />
          </div>
          <div className="flex flex-row items-center gap-4">
            <Label htmlFor="name" className="flex-4">Department</Label>
            <DepartmentLookup setDepartment={setDepartment} department={department}/>           
          </div>        
          <DialogFooter>
            <Button type="submit" disabled={loading}>
              {
                loading && <Spinner className="text-white text-sm"/>
              }
              Invite Employee
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}