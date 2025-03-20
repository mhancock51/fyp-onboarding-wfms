import CommentDTO from '@/models/DTOs/CommentDTO';
import { RootState } from '@/store';
import React, { SetStateAction } from 'react'
import { useSelector } from 'react-redux';
import { Card } from './ui/card';
import { Label } from './ui/label';
import { Separator } from './ui/separator';

interface Props {
  comments: CommentDTO[];
  setParentCommentId: React.Dispatch<SetStateAction<string>>
}

export function GroupedComments(props: Props) {
  function groupComments(comments: CommentDTO[]) {
    const remainingComments = [...comments];
    var groups = [];
    while(remainingComments.length > 0) {
      // find next parent comment
      const parentComment = remainingComments.find(c => !c.parentCommentId)
      if (parentComment === undefined) break;
      // find all child comments 
      const childComments = [...remainingComments.filter(c => c.parentCommentId === parentComment.id)]

      const group = [parentComment, ...childComments];
      // remove reply comments from list
      group.forEach((comment) => {
        const index = remainingComments.indexOf(comment);
        if (index !== -1) remainingComments.splice(index, 1);
      })
      
      groups.push([parentComment, ...childComments]);      
    }
    // sort groups so latest parent comments occur first
    groups = groups.sort((a, b) => (new Date(b[0].creationTimestamp).getTime() - new Date(a[0].creationTimestamp).getTime()))

    return groups;
  }

  const user = useSelector((state: RootState) => state.app.user);

  const groupedComments = groupComments(props.comments);

  return (
    <div className='flex flex-col my-2 max-h-100 overflow overflow-y-auto overflow-x-hidden'>
      {
        groupedComments.map((group, i) => (
          <div key={i} className='flex flex-col gap-2'>
            {/* Render first comment - the parent comment*/}
            <Card key={i} className='flex flex-col gap-1 p-2 my-1 cursor-pointer' onClick={() => {props.setParentCommentId(group[0].id)}}>
              <div className='flex flex-row justify-between p-1'>
                <Label className='font-bold'>{group[0].commentorId === user?.id ? "You" : group[0].accountDirectory.displayName} ({group[0].accountDirectory.departmentName}) said:</Label>
                <Label>({new Date(group[0].creationTimestamp).toLocaleString()})</Label>
              </div>
              <Separator/>
              <Label className='m-1'>{group[0].text}</Label>
            </Card>
            {
              /* Render the child/reply comments */
              group.sort((a, b) => (new Date(a.creationTimestamp).getTime() - new Date(b.creationTimestamp).getTime())).map((comment, j) => (
                <>
                {
                  comment.parentCommentId !== null && comment.parentCommentId !== "" &&
                  <Card key={j} className='flex flex-col gap-1 p-2 ml-6 my-1 bg-accent'>
                    <div className='flex flex-row justify-between p-1'>
                      <Label className='font-bold'>{comment.commentorId === user?.id ? "You" : comment.accountDirectory.displayName} replied:</Label>
                      <Label>({new Date(comment.creationTimestamp).toLocaleString()})</Label>
                    </div>
                    <Separator/>
                    <Label className='m-1'>{comment.text}</Label>
                  </Card>
                }
                </>
              ))
            }
          </div>
        ))
      }
    </div>
  )
}