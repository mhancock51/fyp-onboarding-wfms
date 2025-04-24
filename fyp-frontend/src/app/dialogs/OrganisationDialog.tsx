import Api from '@/api';
import DepartmentLookup from '@/components/Lookups/DepartmentLookup';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { SET_OPEN_ORGANISATION_DIALOG, SET_ORGANISATION } from '@/features/appSlice';
import HTTPresponse from '@/models/HTTPresponse';
import Organisation from '@/models/Organisation';
import { RootState } from '@/store'
import { AxiosResponse } from 'axios';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux'
import { toast } from 'sonner';

export default function OrganisationDialog() {  
  const organisation = useSelector((state: RootState) => state.app.organisation);

  const open = useSelector((state: RootState) => state.app.openOrganisationDialog);
  const dispatch = useDispatch();
  
  const [newName, setNewName] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  function closeAndClear() {
    dispatch(SET_OPEN_ORGANISATION_DIALOG(false));
  }

  async function fetchOrganisation() {
    Api.organisation.fetchOrganisation()
    .then((response: AxiosResponse<HTTPresponse<Organisation, string>>) => {
      dispatch(SET_ORGANISATION(response.data.data));
    })
  }

  async function renameOrganisation() {
    setLoading(true);
    await Api.organisation.renameOrganisation(newName)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success(response.data.data);
      void fetchOrganisation();
    })
    .catch(() => {
      toast.error("Failed to rename organisation");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  useEffect(() => {
    setNewName(organisation?.name ?? "");
  }, [organisation]);

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Manage Organisation</DialogTitle>
          <DialogDescription>
            
          </DialogDescription>
        </DialogHeader>
        <form className="grid gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); void renameOrganisation();}}>
          <div className="grid grid-cols-4 gap-4">
            <Label>Organisation Name</Label>
            <Input required type='text' className="col-span-3" value={newName} onChange={(event: any) => {setNewName(event.target.value);}}/>            
          </div>             
          <div className="grid grid-cols-4 gap-4">
            <Label>Departments</Label>            
            <DepartmentLookup department={null} setDepartment={() => {}}/>
          </div>
          <DialogFooter>
            <Button type="submit">
              {
                loading && <Spinner className="text-primary-foreground"/>
              }
              Update
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
