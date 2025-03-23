import Api from "@/api";
import DepartmentLookup from "@/components/DepartmentLookup";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
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
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Spinner } from "@/components/ui/spinner";
import Department from "@/models/Department";
import { CheckedState } from "@radix-ui/react-checkbox";
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
    setDepartment("");
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
        <div className="grid gap-4 py-4">
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Display Name</Label>
            <Input className="col-span-3" value={displayName} onChange={(event: any) => {setDisplayName(event.target.value);}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Email Address</Label>
            <Input className="col-span-3" value={email} onChange={(event: any) => {setEmail(event.target.value);}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Department</Label>
            <DepartmentLookup setDepartment={setDepartment} department={department}/>           
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Onboarder?</Label>
            <Checkbox className="col-span-3" checked={onboarder} onCheckedChange={(state: CheckedState) => {setOnboarder(state as boolean)}}/>              
          </div>          
          {
            /* Only display if "is onboarder" is selected */
            onboarder &&
            <div>
              <DialogDescription >
                Selected an onboarding workflow for the user to start
              </DialogDescription>
              <div className="grid grid-cols-4 items-center gap-4">
                <Label htmlFor="name" className="text-right">Onboard Workflow</Label>
                <Select>
                  <SelectTrigger className="w-[180px]">
                    <SelectValue placeholder="Select a workflow" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectGroup>
                      <SelectLabel>Onboarding Workflows</SelectLabel>
                      <SelectItem value="test">Workflow 1</SelectItem>
                      <SelectItem value="test2">Workflow 2</SelectItem>
                      <SelectItem value="test3">Workflow 3</SelectItem>       
                    </SelectGroup>
                  </SelectContent>
                </Select> 
              </div>              
            </div>
          }
        </div>
        <DialogFooter>
          <Button type="submit" disabled={loading} onClick={inviteUser}>
            {
              loading && <Spinner className="text-white text-sm"/>
            }
            Invite Employee
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}