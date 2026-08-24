import CommentDTO from "./DTOs/CommentDTO";

export default interface CommentState {
  loadingComments: boolean;
  postingComment: boolean;
  comment: string;
  parentCommentId: string;
  comments: CommentDTO[];
}