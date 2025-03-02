import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import WorkflowTask from "@/models/WorkflowTask";
import React, { useState } from 'react'

export default function AddTaskDialog(props: {open: boolean, onAdd: (task: WorkflowTask) => void, setOpenDialog: React.Dispatch<React.SetStateAction<boolean>>}) {
  const [task, setTask] = useState<WorkflowTask>({ taskId: "", essential: false, assigneeUserId: ""});

  return (
    <Dialog open={props.open} onOpenChange={props.setOpenDialog}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Add Task</DialogTitle>
          <DialogDescription>
            Add a task to your workflow
          </DialogDescription>
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">
              Task Id
            </Label>
            <Input className="col-span-3" value={task.taskId} onChange={(event: any) => {setTask({...task, taskId: event.target.value});}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="username" className="text-right">
              Essential
            </Label>
            <Checkbox value={task.essential ? "true" : "false"} onChange={(event: any) => {setTask({...task, essential: event.target.value});}} />
          </div>
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">
              Assignee user Id
            </Label>
            <Input className="col-span-3" value={task.assigneeUserId} onChange={(event: any) => {setTask({...task, assigneeUserId: event.target.value});}} />
          </div>
        </div>
        <DialogFooter>
          <Button type="submit" onClick={() => {props.onAdd(task);}}>Add Task</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
