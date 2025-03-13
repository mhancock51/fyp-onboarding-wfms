import { WorkflowTemplateNodeDTO } from "../DTOs/WorkflowTemplateNodeDTO";
import WorkflowTemplateNode from "../WorkflowTemplateNode";

export default interface CreateWorkflowTemplatePayload {
  Name: string;
  Description: string;
  IsOnboardingWF: boolean;
  PreflowTasks: WorkflowTemplateNodeDTO[];
  MainflowTasks: WorkflowTemplateNodeDTO[];
}