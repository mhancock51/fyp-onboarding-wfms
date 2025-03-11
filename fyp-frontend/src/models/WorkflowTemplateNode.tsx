import TaskTemplate from "./tasks/TaskTemplate";

export default interface WorkflowTemplateNode {
  id: string;
  taskTemplateId: string;
  assigneeId: string;
  assigneeName: string;
    
}