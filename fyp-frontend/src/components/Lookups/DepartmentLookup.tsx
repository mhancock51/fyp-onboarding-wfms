import React, { useEffect, useState } from 'react'
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import Api from '@/api';
import Department from '@/models/Department';
import { toast } from 'sonner';
import { Button } from '../ui/button';
import { Plus } from 'lucide-react';
import { useDispatch, useSelector } from 'react-redux';
import { SET_DEPARTMENTS, SET_OPEN_CREATE_DPT_DIALOG, SET_OPEN_INVITE_DIALOG, SET_OPEN_ORGANISATION_DIALOG } from '@/features/appSlice';
import { RootState } from '@/store';

interface Props {
  department: Department | null;
  setDepartment: React.Dispatch<React.SetStateAction<Department | null>>;
}

export default function DepartmentLookup(props: Props) {
  const [loading, setLoading] = useState<boolean>(false);  

  const dispatch = useDispatch();
  const departments = useSelector((state: RootState) => state.app.departments);

  async function fetchAllDepartments() {
    setLoading(true);
    Api.fetchDepartments()
    .then((response) => {
      dispatch(SET_DEPARTMENTS(response.data.data as Department[]));      
      setLoading(false);
    })
    .catch(() => {
      toast.error("Failed to load departments");
      setLoading(false);
    });
  }

  useEffect(() => {
    console.log(departments);
    if (departments.length === 0) {
      void fetchAllDepartments();
    }
  }, [departments]);

  return (
    <div className='flex flex-row gap-2 col-span-3 w-auto'>
      <Select value={props.department?.id ?? undefined} onValueChange={(value: string) => {props.setDepartment(departments.find(i => i.id === value) ?? null);}}>
        <SelectTrigger className='w-min-[300px]'>
          <SelectValue placeholder="Select a department" />
        </SelectTrigger>
        <SelectContent>
          {
            !loading &&
            <SelectGroup>
              <SelectLabel>Departments</SelectLabel>
              {
                departments.map((department, index) => (
                  <SelectItem key={index} value={department.id}>{department.displayName}</SelectItem>
                ))
              }          
            </SelectGroup>
          }
          {
            loading &&
            <SelectGroup>
              <SelectLabel>Loading departments</SelectLabel>
            </SelectGroup>
          }
        </SelectContent>
      </Select> 
      <Button type='button' onClick={() => {
        dispatch(SET_OPEN_CREATE_DPT_DIALOG(true)); 
        dispatch(SET_OPEN_ORGANISATION_DIALOG(false));
        dispatch(SET_OPEN_INVITE_DIALOG(false));
      }} variant={"outline"}><Plus/></Button>
    </div>
  )
}
