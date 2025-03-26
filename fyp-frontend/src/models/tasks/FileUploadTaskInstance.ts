import { DateTime } from "luxon";

export default interface FileUploadTaskInstance {
  id: string;
  taskInstanceId: string;
  documentId: string;
  uploadedTimestamp: DateTime | null;
}