import Api from '@/api';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { useState } from 'react';
import { toast } from 'sonner';

export default function DepartmentCreationDialog(props: {open: boolean, setOpenDialog: (open: boolean) => void}) {

  const [name, setName] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  async function createDepartment() {
    setLoading(true);
    await Api.createDepartment(name)
    .then(() => {
      setLoading(false);
      toast(`Successfully created department: ${name}`, { duration: 600, onAutoClose: () => {
        closeAndClear();
      }})
    })
    .catch((error) => {
      setLoading(false);
      const errorMessage = error.response.data.error;
      if (errorMessage === undefined) {
        toast.error(`Failed to create department`);
      }
      else {
        toast.error(`Failed to create department: ${errorMessage}`);
      }      
    })
  }

  function closeAndClear() {
    props.setOpenDialog(false);
  }

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Create a department</DialogTitle>          
        </DialogHeader>
        <div className="grid gap-4 py-4">
          <div className="grid grid-cols-4 items-center gap-4">
            <Label htmlFor="name" className="text-right">Department Name</Label>
            <Input className="col-span-3" value={name} onChange={(event: any) => {setName(event.target.value);}} />
          </div>       
        </div>
        <DialogFooter>
          <Button type="submit" onClick={createDepartment} disabled={loading}>
            <Spinner/>
            Create Department
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
