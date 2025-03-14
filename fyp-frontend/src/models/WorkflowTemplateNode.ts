import AccountDirectory from "./AccountDirectory";
import TaskTemplate from "./tasks/TaskTemplate";

export default interface WorkflowTemplateNode {
  id: string;
  taskTemplate: TaskTemplate | undefined;
  assignee: AccountDirectory | undefined;
  // tasks that must be completed before this task can be started
  taskDependencies: WorkflowTemplateNode[];
}