import React, { useEffect, useState } from 'react'
import DataCard from './DataCard'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table'
import Api from '@/api';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO';
import Utils from '@/util';
import { useSelector } from 'react-redux';
import { RootState } from '@/store';
import { Badge } from '@/components/ui/badge';

export default function OnboardingEmployeesTableCard() {
  const [loading, setLoading] = useState<boolean>(false);
  const [errored, setErrored] = useState<boolean>(false);
  const [instances, setInstances] = useState<WorkflowInstanceDTO[]>([]);

  const departments = useSelector((state: RootState) => state.app.departments);

  async function fetchData() {
    setLoading(true);
    await Api.workflowInstances.fetchAllOpenWorkflowInstance()
    .then((response: AxiosResponse<HTTPresponse<WorkflowInstanceDTO[], string>>) => {
      var instances = (response.data.data as WorkflowInstanceDTO[]).sort((a, b) => (new Date(b.creationTimestamp).getTime() - new Date(a.creationTimestamp).getTime()));      
      setInstances(instances);
    })
    .catch((error) => {
      setErrored(true);
    })
    .finally(() => {
      setLoading(false);
    })
  }

  useEffect(() => {
    void fetchData();
  }, []);

  return (
    <DataCard label={`Onboarding Employees ${instances.length > 0 ? `(${instances.length})` : ""}`} colSpan='md:col-span-2 sm:col-span-3' rowSpan='row-span-2' fontBold='font-normal' 
      fontSize='text-[1.25em]' loading={loading}
      data={
        <div className='max-h-[250px] overflow-y-auto'>
          <Table >
            <TableHeader className='border-b-1'>
              <TableCell width={100} className='text-center'>Employee</TableCell>
              <TableCell width={30}  className='text-center'>Department</TableCell>
              <TableCell width={150} className='text-center'>Workflow</TableCell>
              <TableCell width={50}  className='text-center'>Status</TableCell>
              <TableCell width={50}  className='text-center'>Start Date</TableCell>
            </TableHeader>              
            <TableBody>
              {
                instances.map((instance, index) => (
                  <TableRow key={index}>
                    <TableCell>{instance.onboardingEmployeeDetails?.displayName}</TableCell>
                    <TableCell>{departments.find(d => d.id === instance.onboardingEmployeeDetails?.departmentId)?.displayName}</TableCell>
                    <TableCell>
                      <Badge className='py-2 px-4 w-full rounded-full text-[12px]'>
                        {instance.workflowTemplate.name}
                      </Badge>
                    </TableCell>                
                    <TableCell>
                      <Badge className={`bg-primary py-2 px-4 w-full rounded-full text-[12px] text-primary-foreground flex flex-row gap-2 items-center justify-center ${Utils.getWorkflowStatusColor(instance.status)}`}>
                        {Utils.getWorkflowStatusDisplayName(instance)}
                      </Badge>
                    </TableCell>                  
                    <TableCell>{Utils.dateToDDMMYYYY(instance.creationTimestamp)}</TableCell>
                  </TableRow>
                ))
              }
              </TableBody>
          </Table>
        </div>
      } 
    />
  )
}
