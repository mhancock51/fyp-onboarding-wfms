export interface WorkflowTemplateNodeDTO {
  id: string;
  taskTemplateId: string;
  assigneeId: string;
  dependencyTaskTemplateIds: string[];
}