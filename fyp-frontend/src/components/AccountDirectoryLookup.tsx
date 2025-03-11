import React, { useEffect, useState } from 'react'
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from './ui/select';
import AccountDirectory from '@/models/AccountDirectory';
import Api from '@/api';

interface Props {
  setAccount: React.Dispatch<React.SetStateAction<AccountDirectory | null>>;
  additionalAccounts: AccountDirectory[];
}

export default function AccountDirectoryLookup(props: Props) {
  const [accountsDirectory, setAccountsDirectory] = useState<AccountDirectory[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  async function fetchAccountsDirectory() {
    setLoading(true);
    Api.fetchAccountsDirectory()
    .then((response) => {
      console.log(response);
      setAccountsDirectory(response.data.data);
      setLoading(false);
    })
    .catch((error) => {
      setLoading(false);
    })
  }

  useEffect(() => {
    void fetchAccountsDirectory();
  }, []);

  function handleValueChange(value: string) {
    var account = accountsDirectory.concat(props.additionalAccounts).find(i => i.id === value);
    if (account !== undefined) {
      props.setAccount(account);
    }
  }

  return (
    <Select onValueChange={(value: string) => {handleValueChange(value)}}>
      <SelectTrigger>
        <SelectValue placeholder="Select an Account" />
      </SelectTrigger>
      <SelectContent className="w-full">
        {
          !loading &&
          <SelectGroup>
            <SelectLabel>Accounts</SelectLabel>
            {
              props.additionalAccounts.map((account, index) => (
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
            {
              accountsDirectory.map((account, index) => (
                <SelectItem key={props.additionalAccounts.length + index} value={account.id}>{account.displayName}
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
            <SelectLabel>Loading Accounts</SelectLabel>
          </SelectGroup>
        }
      </SelectContent>
    </Select> 
  )
}
