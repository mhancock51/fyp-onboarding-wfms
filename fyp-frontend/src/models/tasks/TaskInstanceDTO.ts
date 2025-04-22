import WorkflowInstanceDTO from "../DTOs/WorkflowInstanceDTO";
import { ChecklistTaskInstance } from "./ChecklistTaskInstance";
import { FeedbackTaskInstance } from "./FeedbackTaskInstance";
import FileUploadTaskInstance from "./FileUploadTaskInstance";
import ProjectTaskInstance from "./ProjectTaskInstance";
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
    instanceData: ChecklistTaskInstance | ReadDocumentTaskInstance | FileUploadTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null;
    dueDate : Date | null;    
    workflowInstance: WorkflowInstanceDTO | null;
    completionTimestamp: Date | null;
}