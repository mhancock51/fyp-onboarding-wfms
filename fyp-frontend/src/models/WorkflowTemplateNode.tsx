import TaskTemplate from "./tasks/TaskTemplate";

export default interface WorkflowTemplateNode {
  taskTemplateId: string;
  previousTaskTemplateId: string;  
}