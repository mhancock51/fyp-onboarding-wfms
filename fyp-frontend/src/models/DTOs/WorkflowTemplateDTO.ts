import { WorkflowTemplateNodeDTO } from "./WorkflowTemplateNodeDTO";

export default interface WorkflowTemplateDTO {
  id: string;
  name: string;
  description: string;
  isOnboardingWF: boolean;
  preflowNodes: WorkflowTemplateNodeDTO[];
  mainflowNodes: WorkflowTemplateNodeDTO[];
  numberOfTasks: number;
  status: string;
}