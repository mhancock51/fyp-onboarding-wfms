import { ChecklistTaskTemplate } from "./ChecklistTaskTemplate";
import { FeedbackTaskTemplate } from "./FeedbackTaskTemplate";
import { FileUploadTaskTemplate } from "./FileUploadTaskTemplate";
import ProjectTaskTemplate from "./ProjectTaskTemplate";
import { ReadDocumentTaskTemplate } from "./ReadDocumentTaskTemplate";
import TaskType from "./TaskType";

export default interface TaskTemplate {
    id: string;
    name: string;
    description: string;
    creatorAccountId: string;
    dateCreated: string;
    taskTypeId: string;
    taskType: TaskType;
    taskTypeData: FileUploadTaskTemplate | ChecklistTaskTemplate | ReadDocumentTaskTemplate | ProjectTaskTemplate | FeedbackTaskTemplate;
    status: string;
    activeInstances: number;
    lastModifiedTimestamp: Date | null;
}