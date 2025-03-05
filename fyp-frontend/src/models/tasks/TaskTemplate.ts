import { ChecklistTaskTemplate } from "./ChecklistTaskTemplate";
import { FileUploadTaskTemplate } from "./FileUploadTaskTemplate";
import { ReadDocumentTaskTemplate } from "./ReadDocumentTaskTemplate";

export default interface TaskTemplate {
    id: string;
    name: string;
    description: string;
    creatorAccountId: string;
    dateCreated: string;
    taskTypeId: string;
    taskTypeData: FileUploadTaskTemplate | ChecklistTaskTemplate | ReadDocumentTaskTemplate;
}