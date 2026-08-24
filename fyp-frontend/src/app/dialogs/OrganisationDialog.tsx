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
import { ChangeEvent, useEffect, useMemo, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux'
import { toast } from 'sonner';

export default function OrganisationDialog() {  
  const organisation = useSelector((state: RootState) => state.app.organisation);

  const open = useSelector((state: RootState) => state.app.openOrganisationDialog);
  const dispatch = useDispatch();
  
  const [newName, setNewName] = useState<string>("");
  const [selectedLogoFile, setSelectedLogoFile] = useState<File | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  const currentLogoSrc = useMemo(() => {
    if (!organisation?.logoImageData || !organisation?.logoImageMimeType) {
      return null;
    }

    return `data:${organisation.logoImageMimeType};base64,${organisation.logoImageData}`;
  }, [organisation?.logoImageData, organisation?.logoImageMimeType]);

  const selectedLogoPreviewSrc = useMemo(() => {
    if (selectedLogoFile == null) {
      return null;
    }

    return URL.createObjectURL(selectedLogoFile);
  }, [selectedLogoFile]);

  function closeAndClear() {
    dispatch(SET_OPEN_ORGANISATION_DIALOG(false));
    setSelectedLogoFile(null);
  }

  async function fetchOrganisation() {
    Api.organisation.fetchOrganisation()
    .then((response: AxiosResponse<HTTPresponse<Organisation, string>>) => {
      dispatch(SET_ORGANISATION(response.data.data));
    })
  }

  function onSelectLogo(event: ChangeEvent<HTMLInputElement>) {
    const file = event.target.files?.[0] ?? null;
    setSelectedLogoFile(file);
  }

  async function updateOrganisation() {
    const shouldRename = organisation?.name !== newName;
    const shouldUploadLogo = selectedLogoFile !== null;

    if (!shouldRename && !shouldUploadLogo) {
      toast.info("No organisation changes to save");
      return;
    }

    setLoading(true);

    try {
      if (shouldRename) {
        const response: AxiosResponse<HTTPresponse<string, string>> = await Api.organisation.renameOrganisation(newName);
        toast.success(response.data.data);
      }

      if (shouldUploadLogo && selectedLogoFile !== null) {
        const response: AxiosResponse<HTTPresponse<string, string>> = await Api.organisation.uploadOrganisationLogo(selectedLogoFile);
        toast.success(response.data.data);
      }

      await fetchOrganisation();
      setSelectedLogoFile(null);
    }
    catch {
      toast.error("Failed to update organisation details");
    }
    finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    setNewName(organisation?.name ?? "");
  }, [organisation]);

  useEffect(() => {
    return () => {
      if (selectedLogoPreviewSrc != null) {
        URL.revokeObjectURL(selectedLogoPreviewSrc);
      }
    };
  }, [selectedLogoPreviewSrc]);

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle>Manage Organisation</DialogTitle>
          <DialogDescription>
            
          </DialogDescription>
        </DialogHeader>
        <form className="grid gap-4 py-4" onSubmit={(event: any) => {event.preventDefault(); void updateOrganisation();}}>
          <div className="grid grid-cols-4 gap-4">
            <Label>Organisation Name</Label>
            <Input required type='text' className="col-span-3" value={newName} onChange={(event: any) => {setNewName(event.target.value);}}/>            
          </div>             
          <div className="grid grid-cols-4 gap-4 items-center">
            <Label>Logo</Label>
            <div className="col-span-3 flex flex-col gap-2">
              {
                (selectedLogoPreviewSrc ?? currentLogoSrc) !== null &&
                <img
                  src={selectedLogoPreviewSrc ?? currentLogoSrc ?? ""}
                  alt="Organisation logo preview"
                  className="max-h-20 w-auto object-contain"
                />
              }
              <Input type='file' accept='image/*' onChange={onSelectLogo} />
              <p className="text-xs text-muted-foreground">Upload PNG, JPG, SVG or other image types up to 2MB.</p>
            </div>
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
