import OnboardingEmployeeDetailsDTO from "./OnboardingEmployeeDetailsDTO";
import WorkflowTemplateDTO from "./WorkflowTemplateDTO";

export default interface WorkflowInstanceDTO {
  workflowTemplate: WorkflowTemplateDTO;
  completedTasks: number;
  status: string;
  id: string;
  workflowTemplateId: string;
  supervisorAccountId: string;
  creationTimestamp: Date;
  onboardingEmployeeDetails: OnboardingEmployeeDetailsDTO | null;
}