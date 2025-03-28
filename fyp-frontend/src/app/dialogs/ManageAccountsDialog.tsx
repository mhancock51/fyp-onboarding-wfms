import TableActionsDropdown, { DropdownAction } from '@/components/TableActionsDropdown';
import { Button } from '@/components/ui/button';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table';
import { SET_OPEN_ORGANISATION_DIALOG } from '@/features/appSlice';
import AccountDirectory from '@/models/AccountDirectory';
import { RootState } from '@/store';
import { X } from 'lucide-react';
import { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

export default function ManageAccountsDialog() {
  const baseAccounts = useSelector((state: RootState) => state.app.accountsDirectory);
  const open = useSelector((state: RootState) => state.app.openAccountsDialog);
  const dispatch = useDispatch();

  const [searchTerm, setSearchTerm] = useState<string>("");
  const [filteredAccounts, setFilteredAccounts] = useState<AccountDirectory[]>([]);

  const tableActions: DropdownAction[] = [
    {
      label: 'Make supervisor',
      onClick: () => {}
    },
    {
      label: 'Terminate account',
      onClick: () => {}
    }
  ]

  function closeAndClear() {
    dispatch(SET_OPEN_ORGANISATION_DIALOG(false));
  }

  function filterAccounts() {
    if (searchTerm === "") {
      setFilteredAccounts(baseAccounts);
    }
    else {
      const filteredAccounts = baseAccounts.filter(i => i.displayName.startsWith(searchTerm) || i.departmentName.startsWith(searchTerm));
      setFilteredAccounts(filteredAccounts);
    }
  }

  useEffect(() => {
    filterAccounts();
  }, [searchTerm]);

  useEffect(() => {
    filterAccounts();
  }, [baseAccounts]);

  return (
    <Dialog open={open} onOpenChange={closeAndClear}>
      <DialogContent className="sm:max-w-[750px]">
        <DialogHeader>
          <DialogTitle>Manage Accounts</DialogTitle>
        </DialogHeader>
        <div className='p-2 flex flex-col gap-2 max-h-[50vh] overflow-y-auto'>
          <div className='flex flex-row w-full'>
            <Input className='flex-11' type='text' placeholder='Enter account name...' value={searchTerm} onChange={(event: any) => {setSearchTerm(event.target.value)}}/>
            <Button className='flex-1' onClick={() => {setSearchTerm("");}}><X/></Button>            
          </div>
          <Table>
            <TableHeader>
              <TableCell>Name</TableCell>
              <TableCell>Department</TableCell>
              <TableCell>Date Joined</TableCell>
              <TableCell>Supervisor</TableCell>
              <TableCell></TableCell>  
            </TableHeader>
            <TableBody>
              {
                filteredAccounts.map((account, index) => (
                  <TableRow key={index}>
                    <TableCell>{account.displayName}</TableCell>
                    <TableCell>{account.departmentName}</TableCell>
                    <TableCell>[EEE]</TableCell>
                    <TableCell>{account.isSupervisor ? "yes" : "no"}</TableCell>
                    <TableCell>
                      <TableActionsDropdown actions={tableActions}/>
                    </TableCell>
                  </TableRow>

                ))
              }
            </TableBody>
          </Table>
        </div>
      </DialogContent>
    </Dialog>
  )
}
