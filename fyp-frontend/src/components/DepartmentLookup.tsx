import React, { useEffect, useState } from 'react'
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import Api from '@/api';
import Department from '@/models/Department';
import { toast } from 'sonner';
import { Button } from './ui/button';
import { Plus } from 'lucide-react';

export default function DepartmentLookup() {
  const [loading, setLoading] = useState<boolean>(false);
  const [departments, setDepartments] = useState<Department[]>([]);

  async function fetchAllDepartments() {
    Api.fetchDepartments()
    .then((response) => {
      setDepartments(response.data.data as Department[]);
      setLoading(false);
    })
    .catch((error) => {
      toast.error("Failed to load departments");
    });
  }

  useEffect(() => {
    void fetchAllDepartments();
  }, []);

  return (
    <div className='flex flex-row gap-2'>
      <Select>
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select a department" />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectLabel>Departments</SelectLabel>
            {
              departments.map((department, index) => (
                <SelectItem key={index} value={department.departmentId}>{department.displayName}</SelectItem>
              ))
            }          
          </SelectGroup>
        </SelectContent>
      </Select> 
      <Button variant={"outline"}><Plus/></Button>
    </div>
  )
}
