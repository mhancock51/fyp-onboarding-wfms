export interface WorkflowInstanceAuditLog {
  id: string;
  workflowInstanceId: string;
  log: string;
  timestamp: Date;
  accountId: string | null;
}