import { WorkflowTemplateNodeDTO } from "./WorkflowTemplateNodeDTO";
import WorkflowTemplateNode from "../WorkflowTemplateNode";

export default interface WorkflowTemplateDTO {
  Name: string;
  Description: string;
  IsOnboardingWF: boolean;
  PreflowTasks: WorkflowTemplateNodeDTO[];
  MainflowTasks: WorkflowTemplateNodeDTO[];
}