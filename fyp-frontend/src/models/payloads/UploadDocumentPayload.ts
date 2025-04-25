export interface UploadDocumentPayload {
  taskInstanceId: string | null;
  file: File;
  documentName: string;
  accessAccountIds: string[];
}