export interface WorkflowTemplateNodeDTO {
  id: string;
  taskTemplateId: string;
  assigneeId: string;
  dependencyNodeIds: string[];
  daysUntilDue: number | null;
  accountsToNotify: string[];
}