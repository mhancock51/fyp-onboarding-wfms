import { Drawer, DrawerContent, DrawerFooter, DrawerTitle } from '@/components/ui/drawer'
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO'
import TaskTypeBadge from '../TaskTypeBadge';
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
import { Check, Flag, MessageSquareMore, } from 'lucide-react';
import { useDispatch } from 'react-redux';
import CommentSection from '@/components/CommentSection';
import ProjectTask from './ProjectTask';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';
import ProjectTaskInstance from '@/models/tasks/ProjectTaskInstance';
import { SET_OPEN_REPORT_ISSUE_DIALOG } from '@/features/appSlice';
import WorkflowInstanceBadge from '@/components/WorkflowInstanceBadge';
import { TASK_TYPE_IDS } from '@/constants';
import FeedbackTask from './FeedbackTask';
import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance';
import { FeedbackTaskTemplate } from '@/models/tasks/FeedbackTaskTemplate';
import { Spinner } from '@/components/ui/spinner';
import { Progress } from '@/components/ui/progress';
import CommentState from '@/models/CommentState';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  task: TaskInstanceDTO;
  fetchTaskInstances: () => Promise<void>;
}

export default function TaskDrawer(props: Props) {  
  const [canCompleteTask, setCanCompleteTask] = useState<boolean>(false);
  const [loading, setLoading] = useState<boolean>(false);

  const [commentState, setCommentState] = useState<CommentState>({
    loadingComments: false,
    postingComment: false,
    comment: "",
    parentCommentId: "",
    comments: []
  })


  const [updatedTaskInstance, setUpdatedTaskInstance] = useState<ChecklistTaskInstance | ReadDocumentTaskInstance | FileUploadTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null>(null);
  const [instanceHasChanged, setInstanceHasChanged] = useState<boolean>(false);

  const [progress, setProgress] = useState<number>(0);

  const dispatch = useDispatch();

  async function completeTask() {
    setLoading(true);
    await sendUpdateTaskInstance();
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
    .finally(() => {
      setLoading(false);
    })
  }

  async function postComment() {
    setCommentState(prevState => ({...prevState, postingComment: true}));    
    await Api.postTaskTemplateComment(props.task.taskTemplateId, commentState.comment, commentState.parentCommentId)
    .then(() => {    
      setCommentState(prevState => ({...prevState, comment: ""}));      
      void fetchComments();
    })
    .catch(() => {
      toast.error("Failed to post comment");      
    })
    .finally(() => {
      setCommentState(prevState => ({...prevState, postingComment: false}));      
    })
  }

  async function fetchComments() {
    setCommentState(prevState => ({...prevState, loadingComments: true}));    
    await Api.fetchTaskTemplateComments(props.task.taskTemplateId)
    .then((response: AxiosResponse<HTTPresponse<CommentDTO[], string>>) => {
      setCommentState(prevState => ({...prevState, comments: response.data.data}));      
      console.log(response.data.data);
    })
    .catch((error) => {      
      toast.error("Failed to retrieve comments for task template");
    })
    .finally(() => {
      setCommentState(prevState => ({...prevState, loadingComments: false}));
    })
  }

  async function sendUpdateTaskInstance() {
    if (updatedTaskInstance === null) return;
    await Api.updateTaskState(updatedTaskInstance, props.task.template.taskTypeId, props.task.id)
    .then((response) => {
      void props.fetchTaskInstances();
    })
    .catch((error) => {
      toast.error("Failed to update task instance");
    })    
  }

  function updateTaskInstance(taskData: ChecklistTaskInstance | ReadDocumentTaskInstance | FileUploadTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null) {
    setInstanceHasChanged(true)
    setUpdatedTaskInstance(taskData);
  }

  function handleClose() {
    // update task instance state
    if (instanceHasChanged && props.task.status === "open") {
      void sendUpdateTaskInstance();
    }
    props.setOpen(false);
    setCanCompleteTask(false);
  }
  
  useEffect(() => {
    setCommentState(prevState => ({...prevState, comments: [], parentCommentId: ""}));

    if (props.task.taskTemplateId !== "") {
      void fetchComments();
    }
  }, [props.task.taskTemplateId]);

  useEffect(() => {
    setUpdatedTaskInstance(props.task.instanceData);
    setInstanceHasChanged(false);
    setProgress(getProgress(props.task.instanceData));
  }, [props.task]);

  useEffect(() => {
    if (updatedTaskInstance !== null) {
      setProgress(getProgress(updatedTaskInstance));
    }
  }, [updatedTaskInstance]);  

  function getProgress(taskInstanceData: ChecklistTaskInstance | ReadDocumentTaskInstance | FileUploadTaskInstance | ProjectTaskInstance | FeedbackTaskInstance | null) {    
    if (props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.CHECKLIST) {      
      const checkedTasks = ((taskInstanceData || updatedTaskInstance) as ChecklistTaskInstance).itemCompletionStatuses.filter(i => i === true).length;            
      const totalTasks = (props.task.template.taskTypeData as ChecklistTaskTemplate).items.length;       
      const progress = (checkedTasks / totalTasks) * 100;      
      return progress;
    }
    else if (props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.UPLOAD_DOCUMENT) {
      return ((taskInstanceData || updatedTaskInstance) as FileUploadTaskInstance).documentId === "" ? 0 : 100;
    }
    else {
      return 50;
    }
  }

  return (
    <Drawer direction='right'  onClose={handleClose} open={props.open}>
      <DrawerContent className="max-w-[600px] max-h-[100vh] w-full p-2 flex flex-col justify-start gap-2">
        {/* Drawer header - task information */}
        <div className='flex flex-col justify-center gap-2'>
          <DrawerTitle className='text-2xl items-center flex flex-row justify-center'>{props.task.template.name}</DrawerTitle>
          <div className='flex flex-row justify-center gap-2'>
            <TaskTypeBadge taskTypeId={props.task.template.taskTypeId} className='w-[150px]'/>
            {
              props.task.workflowInstance !== null &&
              <WorkflowInstanceBadge workflowInstance={props.task.workflowInstance}/>
            }
            <TaskStatusBadge status={props.task.status} className='w-[100px]'/>
          </div>
          <div className='flex flex-row w-full text-center justify-center'>
            <Label className='font-normal'>{props.task.template.description}</Label>
          </div>                           
        </div>
        <div className='flex-9 flex flex-col min-h-[40vh] gap-2'>
          {
            props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.CHECKLIST &&
            <ChecklistTask 
              taskInstanceId={props.task.id}
              checklistInstance={props.task.instanceData as ChecklistTaskInstance}
              checklistTemplate={props.task.template.taskTypeData as ChecklistTaskTemplate}
              fetchTaskInstances={props.fetchTaskInstances}
              setCanCompleteTask={setCanCompleteTask}
              taskStatus={props.task.status} 
              updateTaskInstance={updateTaskInstance} 
            />
          }
          {
            props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.UPLOAD_DOCUMENT &&
            <UploadDocumentTask 
              taskInstanceId={props.task.id}
              fileUploadInstance={props.task.instanceData as FileUploadTaskInstance}
              fileUploadTemplate={props.task.template.taskTypeData as FileUploadTaskTemplate}
              fetchTaskInstances={props.fetchTaskInstances}
              setCanCompleteTask={setCanCompleteTask}
              taskStatus={props.task.status} 
              updateTaskInstance={updateTaskInstance} 
            />
          }
          {
            props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.READ_DOCUMENT &&
            <ReadDocumentTask 
              taskInstanceId={props.task.id} 
              readDocumentInstance={props.task.instanceData as ReadDocumentTaskInstance} 
              readDocumentTemplate={props.task.template.taskTypeData as ReadDocumentTaskTemplate} 
              fetchTaskInstances={props.fetchTaskInstances} 
              setCanCompleteTask={setCanCompleteTask} 
              taskStatus={props.task.status}
              updateTaskInstance={updateTaskInstance} 
            />
          }
          {
            props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.PROJECT_TASK &&
            <ProjectTask 
              taskInstanceId={props.task.id} 
              projectTemplate={props.task.template.taskTypeData as ProjectTaskTemplate} 
              projectInstance={props.task.instanceData as ProjectTaskInstance}
              fetchTaskInstances={props.fetchTaskInstances} 
              setCanCompleteTask={setCanCompleteTask} 
              taskStatus={props.task.status}
              updateTaskInstance={updateTaskInstance} 
            />
          }
          {
            props.task.template.taskTypeId.toLowerCase() === TASK_TYPE_IDS.FEEDBACK_TASK &&
            <FeedbackTask 
              taskInstanceId={props.task.id} 
              feedbackInstance={props.task.instanceData as FeedbackTaskInstance} 
              feedbackTemplate={props.task.template.taskTypeData as FeedbackTaskTemplate} 
              fetchTaskInstances={props.fetchTaskInstances} 
              setCanCompleteTask={setCanCompleteTask} 
              taskStatus={props.task.status}
              updateTaskInstance={updateTaskInstance}  
            />
          }          
        </div>
        <DrawerFooter className='flex flex-col gap-2 w-full p-0'>
          {
            props.task.status === "open" &&
            <Progress value={progress} className={progress === 100 ? '[&>div]:bg-green-500' : '[&>div]:bg-blue-500'}/>   
          }
          <Button className='rounded-full mx-r-2 p-2 w-full flex flex-row gap-4 items-center' disabled={!canCompleteTask || props.task.status !== "open"} onClick={completeTask}>
            {
              loading &&
              <Spinner className="text-primary-foreground"/>
            }
            <Check/>
            Complete Task
          </Button>          
          <Button variant={"outline"} className='w-full' onClick={() => {dispatch(SET_OPEN_REPORT_ISSUE_DIALOG(true));}}>
            <Flag/>
            Flag an issue with this task
          </Button>
          {/* Task template comment section accordian */}
          <Accordion type="single" collapsible className="w-full flex-10">
            <AccordionItem value="item-1" >
              <AccordionTrigger className="w-full">
                <Button variant={"outline"} className="w-full flex flex-row gap-2">
                  <MessageSquareMore/>
                  See comments about this task
                </Button>              
              </AccordionTrigger>
              <AccordionContent className='py-1'>
                <CommentSection                   
                  loadingComments={commentState.loadingComments} 
                  postingComment={commentState.postingComment} 
                  comments={commentState.comments} 
                  comment={commentState.comment} 
                  parentCommentId={commentState.parentCommentId}
                  setComment={(comment: string) => {setCommentState(prevState => ({...prevState, comment}))}} 
                  setParentCommentId={(parentCommentId: string) => {setCommentState(prevState => ({...prevState, parentCommentId}))}}
                  postComment={postComment}
                />
              </AccordionContent>
            </AccordionItem>
          </Accordion>
          <div className='flex flex-row justify-center w-full py-1'>
            {
              props.task.template.lastModifiedTimestamp !== null &&
              <Label className='font-normal text-gray-500'>Task Template last modified {new Date(props.task.template.lastModifiedTimestamp).toLocaleTimeString()} {new Date(props.task.template.lastModifiedTimestamp).toLocaleDateString()}</Label>                
            }
          </div>
        </DrawerFooter>
      </DrawerContent>
    </Drawer>
  )
}