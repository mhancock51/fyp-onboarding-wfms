import AccountDirectory from "../AccountDirectory";
import TaskInstanceDTO from "../tasks/TaskInstanceDTO";

export default interface DocumentDTO {
  id: string;
  taskInstanceId: string;
  creatorId: string;
  workflowInstanceId: string | null;
  documentData: string;
  fileExtension: string;
  uploadTimestamp: string;
  fileName: string;
  creatorsAccount: AccountDirectory | undefined;
  taskInstance: TaskInstanceDTO | undefined;
}