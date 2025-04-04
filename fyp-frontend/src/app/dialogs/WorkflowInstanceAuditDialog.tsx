import Api from '@/api';
import { Badge } from '@/components/ui/badge';
import { Dialog, DialogContent, DialogTitle } from '@/components/ui/dialog'
import { Table, TableBody, TableCell, TableHeader, TableRow } from '@/components/ui/table';
import WorkflowInstanceDTO from '@/models/DTOs/WorkflowInstanceDTO';
import HTTPresponse from '@/models/HTTPresponse';
import { WorkflowInstanceAuditLog } from '@/models/workflowInstanceAuditLog';
import { RootState } from '@/store';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { useSelector } from 'react-redux';
import { toast } from 'sonner';
import moment from 'moment';
import { Spinner } from '@/components/ui/spinner';
import NoResults from '@/components/NoResults';
import { CircleCheckBig, FileUp, Flag, Rocket, UserPlus } from 'lucide-react';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  workflowInstance: WorkflowInstanceDTO;
}

export default function WorkflowInstanceAuditDialog(props: Props) {
  const accounts = useSelector((state: RootState) => state.app.accountsDirectory);

  const [loading, setLoading] = useState<boolean>(false);
  const [auditLogs, setAuditLogs] = useState<WorkflowInstanceAuditLog[]>([]);

  async function fetchAuditLogs() {
    setLoading(true);
    await Api.audit.fetchWorkflowInstanceAuditLogs(props.workflowInstance.id)
    .then((response: AxiosResponse<HTTPresponse<WorkflowInstanceAuditLog[], string>>) => {
      var logs = (response.data.data as WorkflowInstanceAuditLog[]).sort((a: WorkflowInstanceAuditLog, b: WorkflowInstanceAuditLog) => {return new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()});
      // make workflow started and ended logs be at the beginning and end
      var startLog = logs.find(i => i.log === "Workflow instance started");
      // move start log to beginning
      logs = logs.sort((a: WorkflowInstanceAuditLog, b: WorkflowInstanceAuditLog) => { return a === startLog ? -1 : b === startLog ? 1: 0});      
      var endLog = logs.find(i => i.log === "Workflow instance completed");
      // move end log to end
      logs = logs.sort((a: WorkflowInstanceAuditLog, b: WorkflowInstanceAuditLog) => { return a === endLog ? 1: b === endLog ? 1 : 0});
      if (endLog !== undefined) {
        logs.splice(logs.indexOf(endLog), 1);
        logs.push(endLog);
      }
      setAuditLogs(logs);
    })
    .catch((error) => {
      console.log(error);
      toast.error("Failed to load audit logs");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function closeAndClear() {
    props.setOpen(false);
  }

  function LogDescriptionToIcon(props: {log: string}) {
    const ICON_SIZE = 20;
    if (props.log.toLowerCase() === "workflow instance started") {
      return <Rocket size={ICON_SIZE}/>
    }
    if (props.log.toLowerCase() === "workflow instance completed") {
      return <Flag size={ICON_SIZE}/>
    }
    if (props.log.toLowerCase().includes("uploaded")) {
      return <FileUp size={ICON_SIZE}/>
    }
    if (props.log.toLowerCase().includes("onboarder registered their account") || props.log.toLowerCase().includes("onboarder invited to organisation")) {
      return <UserPlus size={ICON_SIZE}/>
    }
    if (props.log.toLowerCase().includes("task completed")) {
      return <CircleCheckBig size={ICON_SIZE}/>
    }
  }

  useEffect(() => {
    void fetchAuditLogs();
  }, [props.workflowInstance]);

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className='min-w-[850px]'>
        <DialogTitle>Audit Logs</DialogTitle>
        <div className="grid gap-4 py-4 max-h-[75vh] overflow-x-auto"> 
          {
            loading &&
            <div className='flex flex-row w-full justify-center gap-2'>
              <Spinner/>
              Loading audit logs...
            </div> 
          }   
          {
            !loading && auditLogs.length === 0 &&
            <NoResults text={'No logs found'}/>
          }
          {
            !loading && auditLogs.length > 0 &&
            <Table>
              <TableHeader>
                <TableCell className='text-center'>Timestamp</TableCell>
                <TableCell width={1000}>Log</TableCell>
                <TableCell width={100} className='text-center'></TableCell>
                <TableCell width={200} className='text-center'>Account</TableCell>
              </TableHeader>
              <TableBody>
                {
                  auditLogs.map((auditLog, index) => (
                    <TableRow key={index}>
                      <TableCell>
                        {
                          index === auditLogs.length - 1 ? (
                            moment(new Date(auditLog.timestamp)).fromNow()
                          ) : (
                            <>{new Date(auditLog.timestamp).toLocaleTimeString()} {new Date(auditLog.timestamp).toLocaleDateString()}</>
                          )

                        }
                      </TableCell>
                      <TableCell>{auditLog.log}</TableCell>
                      <TableCell className='text-center flex flex-row w-full justify-center items-center h-full'>
                        <div className='py-2'>
                          <LogDescriptionToIcon log={auditLog.log}/>
                        </div>
                      </TableCell>
                      <TableCell className='text-center'>
                        {
                          auditLog.accountId !== null ? (
                            <Badge className='p-2 w-full rounded-full'>
                              {accounts.find(i => i.id === auditLog.accountId)?.displayName}
                            </Badge>
                          ) : (
                            <div className='text-xs'>N/A</div>                            
                          )
                        }
                      </TableCell>
                    </TableRow>
                  ))
                }

              </TableBody>
            </Table>
          }
          <div className='flex flex-col gap-2 w-full'>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  )
}
