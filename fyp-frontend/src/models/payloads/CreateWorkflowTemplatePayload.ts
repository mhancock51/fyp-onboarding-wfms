export interface CreateWorkflowTemplatePayload {
  id: string;
  name: string;
  description: string;
  isOnboardingWF: boolean;
  preflowNodes: CreateWorkflowTemplateNode[];
  mainflowNodes: CreateWorkflowTemplateNode[];
}

export interface CreateWorkflowTemplateNode {
  taskTemplateId: string;
  id: string;
  assigneeId: string;
  dependencyNodeIds: string[];
  daysUntilDue: number | null;
}