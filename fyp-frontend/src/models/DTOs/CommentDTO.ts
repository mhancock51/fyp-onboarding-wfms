import AccountDirectory from "../AccountDirectory";

export default interface CommentDTO {
  accountDirectory: AccountDirectory;
  id: string;
  commentorId: string;
  text: string;
  taskTemplateId: string | null;
  creationTimestamp: string;
  parentCommentId: string | null;
}