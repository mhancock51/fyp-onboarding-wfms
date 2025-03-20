import WorkflowTemplateDTO from "./DTOs/WorkflowTemplateDTO";

export default interface WorkflowInstanceDTO {
  workflowTemplate: WorkflowTemplateDTO;
  completedTasks: number;
  status: string;
  id: string;
  workflowTemplateId: string;
  onboarderAccountId: string | null;
  supervisorAccountId: string;
  creationTimestamp: string;
  onboarderEmailAddress: string | null;
}