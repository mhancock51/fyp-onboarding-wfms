import AccountDirectory from "./AccountDirectory";
import TaskTemplate from "./tasks/TaskTemplate";

export default interface WorkflowTemplateNode {
  id: string;
  taskTemplate: TaskTemplate;
  assignee: AccountDirectory;
  // tasks that must be completed before this task can be started
  taskDependencies: WorkflowTemplateNode[];
}