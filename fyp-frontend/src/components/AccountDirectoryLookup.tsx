import React, { useEffect, useState } from 'react'
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from './ui/select';
import AccountDirectory from '@/models/AccountDirectory';
import Api from '@/api';
import { useDispatch, useSelector } from 'react-redux';
import { SET_ACCOUNTS_DIRECTORY } from '@/features/appSlice';
import { RootState } from '@/store';

interface Props {
  account: AccountDirectory | null;
  setAccount: React.Dispatch<React.SetStateAction<AccountDirectory | null>>;
  additionalAccounts: AccountDirectory[];
  filter?: (a: AccountDirectory) => boolean;
}

export default function AccountDirectoryLookup(props: Props) {  
  const [loading, setLoading] = useState<boolean>(false);

  const dispatch = useDispatch();
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);

  const filteredAccounts = props.filter !== undefined ? props.additionalAccounts.concat(accounts).filter(props.filter) : props.additionalAccounts.concat(accounts);

  async function fetchAccountsDirectory() {
    setLoading(true);
    Api.fetchAccountsDirectory()
    .then((response) => {      
      var accountsDirectory = response.data.data as AccountDirectory[];           
      dispatch(SET_ACCOUNTS_DIRECTORY(accountsDirectory));
          
    })
    .catch((error) => {
      setLoading(false);
    })
  }

  useEffect(() => {
    if (accounts.length === 0) {
      void fetchAccountsDirectory();
    }
  }, []);

  function handleValueChange(value: string) {
    var account = accounts.concat(props.additionalAccounts).find(i => i.id === value);
    if (account !== undefined) {
      props.setAccount(account);
    }
  }

  return (
    <Select onValueChange={(value: string) => {handleValueChange(value)}} value={props.account?.id ?? undefined}>
      <SelectTrigger className='flex-8 col-span-3'>
        <SelectValue placeholder="Select an Account" />
      </SelectTrigger>
      <SelectContent className="w-full">
        {
          !loading &&
          <SelectGroup>
            <SelectLabel>Accounts</SelectLabel>
            {
              filteredAccounts.map((account, index) => (
                <SelectItem key={index} value={account.id}>{account.displayName}
                {
                  account.departmentName !== "" &&
                  <>
                    {" "}({account.departmentName})
                  </>
                } 
                </SelectItem>
              ))
            }        
          </SelectGroup>
        }
        {
          loading &&
          <SelectGroup>
            <SelectLabel>Loading Accounts...</SelectLabel>
          </SelectGroup>
        }
      </SelectContent>
    </Select> 
  )
}
