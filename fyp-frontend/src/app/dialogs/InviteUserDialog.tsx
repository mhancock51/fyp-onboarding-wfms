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
import { Separator } from "@/components/ui/separator";
import WorkflowTask from "@/models/WorkflowTask";
import { CheckedState } from "@radix-ui/react-checkbox";
import { useState } from "react";

export default function InviteUserDialog(props: {open: boolean, setOpenDialog: (open: boolean) => void}) {
  const [onboarder, setOnboarder] = useState<boolean>(false);

  return (
    <Dialog open={props.open} onOpenChange={props.setOpenDialog}>
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
            <Input className="col-span-3" value={""} onChange={(event: any) => {}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Email Address</Label>
            <Input className="col-span-3" value={""} onChange={(event: any) => {}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Department</Label>
            <DepartmentLookup/>           
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Onboarder?</Label>
            <Checkbox className="col-span-3" checked={onboarder} onCheckedChange={(state: CheckedState) => {setOnboarder(state as boolean)}}/>              
          </div>
          {
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
          <Button type="submit">Invite Employee</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}