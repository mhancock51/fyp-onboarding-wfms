import { WorkflowTemplateNodeDTO } from "./WorkflowTemplateNodeDTO";

export default interface WorkflowTemplateDTO {
  Id: string;
  Name: string;
  Description: string;
  IsOnboardingWF: boolean;
  PreflowTasks: WorkflowTemplateNodeDTO[];
  MainflowTasks: WorkflowTemplateNodeDTO[];
}