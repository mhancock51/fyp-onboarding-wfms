export interface CreateWorkflowTemplatePayload {
  id: string;
  name: string;
  description: string;
  isOnboardingWF: boolean;
  preflowTasks: CreateWorkflowTemplateNode[];
  mainflowTasks: CreateWorkflowTemplateNode[];
}

export interface CreateWorkflowTemplateNode {
  taskTemplateId: string;
  id: string;
  assigneeId: string;
  dependencyNodeIds: string[];
}