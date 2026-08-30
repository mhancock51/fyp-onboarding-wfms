import AccountDirectory from "../AccountDirectory";
import TaskTemplate from "../tasks/TaskTemplate";
import Department from "../Department";

export default interface WorkflowTemplateNode {
  id: string;
  taskTemplate: TaskTemplate | undefined;
  assignee: AccountDirectory | undefined;
  department?: Department;
  // tasks that must be completed before this task can be started
  taskDependencies: WorkflowTemplateNode[];
  daysUntilDue: number | null;
  accountsToNotify: string[];
}