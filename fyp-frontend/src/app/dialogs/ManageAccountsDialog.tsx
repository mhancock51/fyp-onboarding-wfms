import Api from '@/api';
import NoResults from '@/components/NoResults';
import TableActionsDropdown, { DropdownAction } from '@/components/TableActionsDropdown';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Spinner } from '@/components/ui/spinner';
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table';
import { SET_ACCOUNTS_DIRECTORY, SET_OPEN_ACCOUNTS_DIALOG, SET_OPEN_ORGANISATION_DIALOG } from '@/features/appSlice';
import AccountDirectory from '@/models/AccountDirectory';
import HTTPresponse from '@/models/HTTPresponse';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import { X } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { toast } from 'sonner';

export default function ManageAccountsDialog() {
  const baseAccounts = useSelector((state: RootState) => state.app.accountsDirectory);
  const open = useSelector((state: RootState) => state.app.openAccountsDialog);
  const dispatch = useDispatch();

  const [searchTerm, setSearchTerm] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [filteredAccounts, setFilteredAccounts] = useState<AccountDirectory[]>([]);

  async function fetchAccounts() {
    setLoading(true);
    Api.fetchAccountsDirectory()
    .then((response: AxiosResponse<HTTPresponse<AccountDirectory[], string>>) => {
      dispatch(SET_ACCOUNTS_DIRECTORY(response.data.data));
    })
    .catch((error) => {
      toast.error("Failed to fetch accounts");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function makeSupervisor(accountId: string) {
    await Api.account.makeSupervisor(accountId)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      toast.success(response.data.data);
      void fetchAccounts();
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to grant supervisor status");
      }      
    })
  }

  function closeAndClear() {
    dispatch(SET_OPEN_ACCOUNTS_DIALOG(false));
  }

  function filterAccounts() {
    if (searchTerm === "") {
      setFilteredAccounts(baseAccounts);
    }
    else {
      const filteredAccounts = baseAccounts.filter(i => 
        i.displayName.toLowerCase().startsWith(searchTerm.toLowerCase()) || 
        i.departmentName.toLowerCase().startsWith(searchTerm.toLowerCase()) || 
        i.emailAddress.toLowerCase().startsWith(searchTerm.toLowerCase())
      );
      setFilteredAccounts(filteredAccounts);
    }
  }

  useEffect(() => {
    filterAccounts();
  }, [searchTerm]);

  useEffect(() => {
    filterAccounts();
  }, [baseAccounts]);

  useEffect(() => {
    if (baseAccounts.length === 0) {
      void fetchAccounts();
    }
  }, []);

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[800px]">
        <DialogHeader>
          <DialogTitle>Manage Accounts</DialogTitle>
        </DialogHeader>
        <div className='p-2 flex flex-col gap-2'>
          <div className='flex flex-row w-full gap-2'>
            <Input disabled={loading} className='flex-11' type='text' placeholder='Enter account name...' value={searchTerm} onChange={(event: any) => {setSearchTerm(event.target.value)}}/>
            <Button className='flex-1' onClick={() => {setSearchTerm("");}}><X/></Button>            
          </div>
          {
            loading &&
            <div className='flex flex-row justify-center w-full gap-2'>
              <Spinner/>
              Loading accounts...
            </div>
          }
          {
            filteredAccounts.length > 0 && !loading &&
            <div className='flex flex-col gap-1 h-[50vh] overflow-y-auto'>
              <Label>Showing {filteredAccounts.length} accounts</Label>
              <Table>
                <TableHeader>
                  <TableCell>Name</TableCell>
                  <TableCell>Email Address</TableCell>
                  <TableCell>Department</TableCell>
                  <TableCell>Supervisor</TableCell>
                  <TableCell></TableCell>  
                </TableHeader>
                <TableBody>
                  {
                    filteredAccounts.map((account, index) => (
                      <TableRow key={index}>
                        <TableCell>{account.displayName}</TableCell>
                        <TableCell>{account.emailAddress}</TableCell>
                        <TableCell>{account.departmentName}</TableCell>
                        <TableCell>{account.isSupervisor ? "yes" : "no"}</TableCell>
                        <TableCell>
                          <TableActionsDropdown actions={[
                            {
                              label: 'Make Supervisor',
                              onClick: () => {void makeSupervisor(account.id)}
                            }
                          ]}/>
                        </TableCell>
                      </TableRow>

                    ))
                  }
                </TableBody>
              </Table>
            </div>
          }
          {
            filteredAccounts.length === 0 && !loading &&
            <NoResults text={'No accounts found'}/>
          }
        </div>
      </DialogContent>
    </Dialog>
  )
}
