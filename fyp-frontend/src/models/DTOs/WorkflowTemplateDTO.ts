import { WorkflowTemplateNodeDTO } from "./WorkflowTemplateNodeDTO";

export default interface WorkflowTemplateDTO {
  id: string;
  name: string;
  description: string;
  isOnboardingWF: boolean;
  preflowTasks: WorkflowTemplateNodeDTO[];
  mainflowTasks: WorkflowTemplateNodeDTO[];
  numberOfTasks: number;
}