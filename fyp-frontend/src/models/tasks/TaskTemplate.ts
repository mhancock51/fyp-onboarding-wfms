import { ChecklistTaskTemplate } from "./ChecklistTaskTemplate";
import { FileUploadTaskTemplate } from "./FileUploadTaskTemplate";
import { ReadDocumentTaskTemplate } from "./ReadDocumentTaskTemplate";
import TaskType from "./taskType";

export default interface TaskTemplate {
    id: string;
    name: string;
    description: string;
    creatorAccountId: string;
    dateCreated: string;
    taskTypeId: string;
    taskType: TaskType;
    taskTypeData: FileUploadTaskTemplate | ChecklistTaskTemplate | ReadDocumentTaskTemplate;
}