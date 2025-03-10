export interface Document {
  id: string;
  taskInstanceId: string;
  creatorId: string;
  workflowInstanceId: string;
  documentData: string;
  fileExtension: string;
  uploadTimestamp: string;
  fileName: string;
}