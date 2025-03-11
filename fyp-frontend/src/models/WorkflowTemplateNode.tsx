import AccountDirectory from "./AccountDirectory";
import TaskTemplate from "./tasks/TaskTemplate";

export default interface WorkflowTemplateNode {
  id: string;
  taskTemplate: TaskTemplate;
  assignee: AccountDirectory;
}