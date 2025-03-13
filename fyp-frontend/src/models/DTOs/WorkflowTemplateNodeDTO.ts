export interface WorkflowTemplateNodeDTO {
  Id: string;
  TaskTemplateId: string;
  AssigneeId: string;
  DependencyTaskTemplateIds: string[];
}