import Api from '@/api';
import TableActionsDropdown from '@/components/TableActionsDropdown';
import { Badge } from '@/components/ui/badge';
import { Label } from '@/components/ui/label';
import { Separator } from '@/components/ui/separator'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table';
import IssueDTO from '@/models/DTOs/IssueDTO'
import HTTPresponse from '@/models/HTTPresponse';
import Utils from '@/util';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';

export default function IssuesPage() {
  const [issues, setIssues] = useState<IssueDTO[]>([]);
  const [loading, setLoading] = useState<boolean>(false);

  async function fetchIssues() {
    setLoading(true);
    Api.issues.fetchAll()
    .then((response: AxiosResponse<HTTPresponse<IssueDTO[], string>>) => {
      setIssues(response.data.data);
    })
    .catch((error) => {
      toast.error("Failed to load issues");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  useEffect(() => {
    void fetchIssues();
  }, []);

  return (
    <div>
      <div>
        <h1 className='text-xl text-foreground font-bold m-2'>Reported Issues</h1>
        <Separator/>
        <div className='flex flex-col w-full p-2'>
          <Label>Open Issues: {issues.filter(i => i.status === "open").length}</Label>
          <Table>
            <TableHeader>
              <TableCell width={50}>Id</TableCell>
              <TableCell className='text-center' width={100}>Task Template</TableCell>
              <TableCell className='text-center' width={75}>Creator</TableCell>              
              <TableCell className='text-center' width={75}>Status</TableCell>
              <TableCell className='text-center' width={75}>Logged</TableCell>
              <TableCell width={1200}>Description</TableCell>
              <TableCell></TableCell>
            </TableHeader>  
            <TableBody>
              {
                issues.map((issue, index) => (
                  <TableRow key={index}>
                    <TableCell>{issue.id}</TableCell>
                    <TableCell>
                      <Badge className='p-2 rounded-full min-w-[200px]'>
                        {issue.taskInstance.template.name}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Badge className='p-2 rounded-full min-w-[150px]'>{issue.issueCreatorAccount.displayName}</Badge>  
                    </TableCell>    
                    <TableCell>
                      <Badge className='p-2 rounded-full min-w-[100px] bg-blue-500'>
                        {issue.status}
                      </Badge>
                    </TableCell>
                    <TableCell>{new Date(issue.issueLoggedTimestamp).toLocaleDateString()}</TableCell>
                    <TableCell>
                      {issue.description}
                    </TableCell>                
                    <TableCell>
                      <TableActionsDropdown actions={[
                        {
                          label: 'Update Status',
                          onClick: () => {}
                        },
                        {
                          label: 'Edit Task Template',
                          onClick: () => {}
                        }
                      ]}/>
                    </TableCell>
                  </TableRow>
                ))
              }
            </TableBody>
          </Table>          
        </div>
      </div>
    </div>
  )
}
