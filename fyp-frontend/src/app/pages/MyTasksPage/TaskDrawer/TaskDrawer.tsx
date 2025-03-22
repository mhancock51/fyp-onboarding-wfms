import { Drawer, DrawerClose, DrawerContent, DrawerDescription, DrawerFooter, DrawerHeader, DrawerTitle, DrawerTrigger } from '@/components/ui/drawer'
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO'
import TaskTypeBadge from '../TaskTypeBadge';
import { Separator } from '@/components/ui/separator';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { FileUploadTaskTemplate } from '@/models/tasks/FileUploadTaskTemplate';
import { ReadDocumentTaskTemplate } from '@/models/tasks/ReadDocumentTaskTemplate';
import ChecklistTask from './ChecklistTask';
import { ChecklistTaskInstance } from '@/models/tasks/ChecklistTaskInstance';
import { ChecklistTaskTemplate } from '@/models/tasks/ChecklistTaskTemplate';
import { SetStateAction, useEffect, useState } from 'react';
import Api from '@/api';
import { toast } from 'sonner';
import ReadDocumentTask from './ReadDocumentTask';
import ReadDocumentTaskInstance from '@/models/tasks/ReadDocumentTaskInstance';
import TaskStatusBadge from '../TaskStatusBadge';
import UploadDocumentTask from './UploadDocumentTask';
import FileUploadTaskInstance from '@/models/tasks/FileUploadTaskInstance';
import { Accordion, AccordionContent, AccordionItem } from '@/components/ui/accordion';
import { AccordionTrigger } from '@radix-ui/react-accordion';
import { AxiosResponse } from 'axios';
import HTTPresponse from '@/models/HTTPresponse';
import CommentDTO from '@/models/DTOs/CommentDTO';
import { Spinner } from '@/components/ui/spinner';
import { Input } from '@/components/ui/input';
import { Card } from '@/components/ui/card';
import NoResults from '@/components/NoResults';
import { Flag, MessageSquareMore, X } from 'lucide-react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '@/store';
import CommentSection from '@/components/CommentSection';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  task: TaskInstanceDTO;
  fetchTaskInstances: () => Promise<void>;
}

export default function TaskDrawer(props: Props) {  
  const [canCompleteTask, setCanCompleteTask] = useState<boolean>(false);
  const [comments, setComments] = useState<CommentDTO[]>([]);

  const [loadingComments, setLoadingComments] = useState<boolean>(false);
  const [postingComment, setPostingComment] = useState<boolean>(false);

  const [comment, setComment] = useState<string>("");
  const [parentCommentId, setParentCommentId] = useState<string>("");

  const user = useSelector((state: RootState) => state.app.user);

  const dispatch = useDispatch();

  async function completeTask() {
    if (!canCompleteTask) return;    
    await Api.completeTask(props.task.id)
    .then((response) => {
      console.log(response);
      toast("Successfully completed task");
      void props.fetchTaskInstances();
      props.setOpen(false);
    })
    .catch((error) => {
      toast.error("Failed to complete task");
      console.error(error);
    })
  }

  async function postComment() {
    setPostingComment(true);
    await Api.postTaskTemplateComment(props.task.taskTemplateId, comment, parentCommentId)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {      
      setComment("");
      void fetchComments();
    })
    .catch((error) => {
      toast.error("Failed to post comment");
    })
    .finally(() => {
      setPostingComment(false);
    })
  }

  async function fetchComments() {
    setLoadingComments(true);
    await Api.fetchTaskTemplateComments(props.task.taskTemplateId)
    .then((response: AxiosResponse<HTTPresponse<CommentDTO[], string>>) => {
      setComments(response.data.data);
    })
    .catch((error) => {
      console.error("EEE:", error);
      toast.error("Failed to retrieve comments for task template");
    })
    .finally(() => {
      setLoadingComments(false);
    })
  }
  
  useEffect(() => {
    if (props.task.taskTemplateId !== "") {
      void fetchComments();
    }
  }, [props.task.taskTemplateId]);

  return (
    <Drawer direction='right'  onClose={() => {props.setOpen(false);}} open={props.open}>
      <DrawerContent className="max-w-[600px] w-full p-2"> {/* Override max width */}
        {
          props.task !== null &&
          <>
          <DrawerHeader className='p-3'>
            <div className='flex flex-col justify-center gap-1'>
              <DrawerTitle className='text-2xl items-center flex flex-row justify-center'>{props.task.template.name}</DrawerTitle>
              {
                user?.isAdmin &&
                <Badge className='mx-auto text-center cursor-pointer' onClick={() => {navigator.clipboard.writeText(props.task.id);}}>
                  [{props.task.id}]
                </Badge>              
              }
            </div>
            <div className='flex flex-row justify-center' style={{gap: "2px"}}>
              <TaskTypeBadge taskTypeId={props.task.template.taskTypeId}/>
              {
                props.task.workflowInstanceId &&
                <Badge className='mx-2 py-2 px-4 rounded-full'>
                  {props.task.workflowInstanceTemplateName === "" ? "N/A" : props.task.workflowInstanceTemplateName}
                </Badge>
              }
              <TaskStatusBadge status={props.task.status}/>
            </div>
            <Separator/>            
            <DrawerDescription>{props.task.template.description}</DrawerDescription>
          </DrawerHeader>
          {
            props.task.template.taskTypeId.toLowerCase() === "checklist" &&
            <ChecklistTask 
              taskInstanceId={props.task.id} 
              checklistInstance={props.task.instanceData as ChecklistTaskInstance} 
              checklistTemplate={props.task.template.taskTypeData as ChecklistTaskTemplate} 
              fetchTaskInstances={props.fetchTaskInstances}
              setCanCompleteTask={setCanCompleteTask}
              taskStatus={props.task.status}
            />
          }
          {
            props.task.template.taskTypeId.toLowerCase() === "upload-document" &&
            <UploadDocumentTask 
              taskInstanceId={props.task.id} 
              fileUploadInstance={props.task.instanceData as FileUploadTaskInstance} 
              fileUploadTemplate={props.task.template.taskTypeData as FileUploadTaskTemplate} 
              fetchTaskInstances={props.fetchTaskInstances} 
              setCanCompleteTask={setCanCompleteTask} 
              taskStatus={props.task.status}/>
          }
          {
            props.task.template.taskTypeId.toLowerCase() === "read-document" &&
            <ReadDocumentTask 
              taskInstanceId={props.task.id} 
              readDocumentInstance={props.task.instanceData as ReadDocumentTaskInstance} 
              readDocumentTemplate={props.task.template.taskTypeData as ReadDocumentTaskTemplate} 
              fetchTaskInstances={props.fetchTaskInstances} 
              setCanCompleteTask={setCanCompleteTask} 
              taskStatus={props.task.status}/>
          }
          </>
        }
        <Button className='rounded-full mx-2 p-2' disabled={!canCompleteTask || props.task.status !== "open"} onClick={completeTask}>
          Complete Task
        </Button>
        <DrawerFooter>
          <Button variant={"outline"}>
            <Flag/>
            Flag an issue with this task
          </Button>
          <Accordion type="single" collapsible className="w-full">
            <AccordionItem value="item-1" >
              <AccordionTrigger className="w-full">
                <Button variant={"outline"} className="w-full flex flex-row gap-2">
                  <MessageSquareMore/>
                  See comments about this task
                </Button>              
              </AccordionTrigger>
              <AccordionContent className='p-2'>
                <CommentSection 
                  loadingComments={loadingComments} 
                  postingComment={postingComment} 
                  comments={comments} 
                  comment={comment} 
                  parentCommentId={parentCommentId}
                  setComment={setComment} 
                  setParentCommentId={setParentCommentId}
                  postComment={postComment}
                />
              </AccordionContent>
            </AccordionItem>
          </Accordion>
          
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  )
}