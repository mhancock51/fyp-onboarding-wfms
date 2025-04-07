import { ChecklistTaskTemplate } from "./ChecklistTaskTemplate";
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
    taskTypeData: FileUploadTaskTemplate | ChecklistTaskTemplate | ReadDocumentTaskTemplate | ProjectTaskTemplate;
    status: string;
    activeInstances: number;
}