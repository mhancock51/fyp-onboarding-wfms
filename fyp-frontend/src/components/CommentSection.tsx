import CommentDTO from '@/models/DTOs/CommentDTO';
import React, { SetStateAction, useEffect, useState } from 'react'
import { Spinner } from './ui/spinner';
import { Label } from './ui/label';
import NoResults from './NoResults';
import { Button } from './ui/button';
import { X } from 'lucide-react';
import { Input } from './ui/input';
import { GroupedComments } from './GroupedComments';

interface Props {
  loadingComments: boolean; 
  postingComment: boolean;
  comments: CommentDTO[];
  comment: string;
  parentCommentId: string;
  setComment: React.Dispatch<SetStateAction<string>>;
  setParentCommentId: React.Dispatch<SetStateAction<string>>;
  postComment: () => Promise<void>;
}

export default function CommentSection(props: Props) {  
  const [parentComment, setParentComment] = useState<CommentDTO | null>(null);

  useEffect(() => {
    setParentComment(props.comments.find(c => c.id === props.parentCommentId) ?? null);
  }, [props.parentCommentId]);

  return (
    <div className='flex flex-col gap-2'>
      {
        props.loadingComments &&
        <div className='flex flex-row py-2 gap-4 items-center justify-center'>
          <Spinner/>
          <Label>Loading comments...</Label>
        </div>
      }
      {
        !props.loadingComments && props.comments.length === 0 &&
        <NoResults text={'No comments on this task template yet'}/>
      }
      {
        !props.loadingComments && props.comments.length > 0 &&
        <div className='flex flex-col'>
          <h1>Click on a comment to reply</h1>
          <GroupedComments comments={props.comments} setParentCommentId={props.setParentCommentId}/>
          <div className='flex flex-col gap-2'>
            {
              parentComment &&
              <div className='flex flex-row justify-between items-center'>
                <span className='text-ellipsis w-100 whitespace-nowrap overflow-x-hidden'>Replying to {parentComment.accountDirectory.displayName}'s comment: "{parentComment.text}"</span>
                <Button onClick={() => {props.setParentCommentId("");}}><X/></Button>
              </div>
            }
            <form className='flex flex-row gap-2' onSubmit={(event: any) => {event.preventDefault(); void props.postComment();}}>
              <Input type='text' disabled={props.postingComment} placeholder='Enter your comment...' onChange={(event: any) => {props.setComment(event.target.value);}}/>
              <Button disabled={props.comment === "" || props.postingComment} type='submit' className='flex flex-row gap-2'>
                {
                  props.postingComment &&
                  <Spinner className="text-primary-foreground"/>
                }
                Submit
              </Button>
            </form>
          </div> 
        </div>
      }
    </div>
  )
}