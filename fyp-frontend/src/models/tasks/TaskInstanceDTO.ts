import { ChecklistTaskInstance } from "./ChecklistTaskInstance";
import FileUploadTaskInstance from "./FileUploadTaskInstance";
import ReadDocumentTaskInstance from "./ReadDocumentTaskInstance";
import TaskTemplate from "./TaskTemplate";

export default interface TaskInstanceDTO {
    id: string;
    assigneeAccountId: string;
    assignerAccountId: string;
    taskTemplateId: string;
    creationTimestamp: string;
    status: string;
    template: TaskTemplate;
    workflowInstanceTemplateName: string;
    workflowInstanceId?: string;
    instanceData: ChecklistTaskInstance | ReadDocumentTaskInstance | FileUploadTaskInstance | null;
    dueDate : Date | null;
}