import { ChecklistTaskInstance } from "./ChecklistTaskInstance";
import ReadDocumentTaskInstance from "./ReadDocumentTaskInstance";
import TaskTemplate from "./TaskTemplate";

export default interface TaskInstance {
    id: string;
    assigneeAccountId: string;
    assignerAccountId: string;
    taskTemplateId: string;
    creationTimestamp: string;
    status: string;
    template: TaskTemplate;
    instanceData: ChecklistTaskInstance | ReadDocumentTaskInstance | null;
}