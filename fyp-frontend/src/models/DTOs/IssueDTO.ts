import AccountDirectory from "../AccountDirectory";
import TaskInstanceDTO from "../tasks/TaskInstanceDTO";

export default interface IssueDTO {
  id: string;
  taskTemplateId: string;
  taskInstanceId: string;
  issueCreatorId: string;
  issueLoggedTimestamp: Date;
  description: string;
  suggestedChanges: string;
  status: string;
  issueCreatorAccount: AccountDirectory;
  taskInstance: TaskInstanceDTO;
}