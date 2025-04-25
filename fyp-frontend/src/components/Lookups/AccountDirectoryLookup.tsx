import React, { useEffect, useState } from 'react';
import AccountDirectory from '@/models/AccountDirectory';
import Api from '@/api';
import { useDispatch, useSelector } from 'react-redux';
import { SET_ACCOUNTS_DIRECTORY } from '@/features/appSlice';
import { RootState } from '@/store';
import Select, { ActionMeta, MultiValue, SingleValue } from 'react-select';
import { PLACEHOLDER_ONBOARDERS_ACCOUNT, PLACEHOLDER_SUPERVISORS_ACCOUNT } from '@/constants';

interface Props {
  accounts: AccountDirectory[] | [];
  setAccounts: (accounts: AccountDirectory[]) => void;
  additionalAccounts: AccountDirectory[];
  filter?: (a: AccountDirectory) => boolean;
  isMulti: boolean;
}

export default function AccountDirectoryLookup(props: Props) {
  const [loading, setLoading] = useState<boolean>(false);

  const dispatch = useDispatch();
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);

  const filteredAccounts =
    props.filter !== undefined
      ? props.additionalAccounts.concat(accounts).filter(props.filter)
      : props.additionalAccounts.concat(accounts);

  function getAccountsDisplayString(account: AccountDirectory): string {
    if (
      account.id !== PLACEHOLDER_ONBOARDERS_ACCOUNT.id &&
      account.id !== PLACEHOLDER_SUPERVISORS_ACCOUNT.id
    ) {
      return `${account.displayName} (${account.departmentName})`;
    } else {
      return account.displayName;
    }
  }

  function getSelectOptions(): { label: string; value: string }[] {
    return filteredAccounts.map((account) => ({
      label: getAccountsDisplayString(account),
      value: account.id,
    }));
  }

  function getSelectValue(): any {
    const options = getSelectOptions();

    if (props.isMulti) {
      return options.filter((option) =>
        props.accounts.some((a) => a.id === option.value)
      );
    } else {
      return options.find((option) => option.value === props.accounts[0]?.id) || null;
    }
  }

  function handleValueChange(
    newValue: MultiValue<{ label: string; value: string }> | SingleValue<{ label: string; value: string }>,
    actionMeta: ActionMeta<{ label: string; value: string }>
  ): void {
    if (props.isMulti) {
      const selectedIds = (newValue as MultiValue<{ label: string; value: string }>).map((v) => v.value);
      const selectedAccounts = filteredAccounts.filter((a) => selectedIds.includes(a.id));
      props.setAccounts(selectedAccounts);
    } else {
      const selectedId = (newValue as SingleValue<{ label: string; value: string }>)?.value;
      const selectedAccount = filteredAccounts.find((a) => a.id === selectedId);
      props.setAccounts(selectedAccount ? [selectedAccount] : []);
    }
  }

  async function fetchAccountsDirectory(): Promise<void> {
    setLoading(true);
    try {
      const response = await Api.fetchAccountsDirectory();
      const accountsDirectory = response.data.data as AccountDirectory[];
      dispatch(SET_ACCOUNTS_DIRECTORY(accountsDirectory));
    } catch (error) {
      // Optional error handling
    } finally {
      setLoading(false);
    }
  }

  useEffect(function () {
    if (accounts.length === 0) {
      void fetchAccountsDirectory();
    }
  }, []);

  return (
    <Select
      className='col-span-3'
      isMulti={props.isMulti}
      options={getSelectOptions()}
      value={getSelectValue()}
      isLoading={loading}
      onChange={handleValueChange}
    />
  );
}
