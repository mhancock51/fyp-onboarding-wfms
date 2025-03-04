import { ChecklistTaskTemplate } from "./ChecklistTaskTemplate";
import { FileUploadTaskTemplate } from "./FileUploadTaskTemplate";
import { ReadDocumentTaskTemplate } from "./ReadDocumentTaskTemplate";

export default interface TaskTemplate {
    taskTemplateId: string;
    name: string;
    description: string;
    creatorAccountId: string;
    dateCreated: string;
    taskTypeId: string;
    taskTypeData: FileUploadTaskTemplate | ChecklistTaskTemplate | ReadDocumentTaskTemplate;
}