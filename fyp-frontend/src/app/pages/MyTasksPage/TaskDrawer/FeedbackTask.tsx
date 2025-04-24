import Api from '@/api';
import { Label } from '@/components/ui/label';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Separator } from '@/components/ui/separator';
import { TASK_TYPE_IDS } from '@/constants';
import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance';
import { FeedbackTaskTemplate } from '@/models/tasks/FeedbackTaskTemplate';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';

interface Props {
  taskInstanceId: string;
  feedbackInstance: FeedbackTaskInstance;
  feedbackTemplate: FeedbackTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
}

export default function FeedbackTask(props: Props) {
  const [feedbackState, setFeedbackState] = useState<FeedbackTaskInstance>(props.feedbackInstance);

  function areAllQuestionsAnswered() {
    return feedbackState.responses.includes(-1) ? false : true;
  }

  async function updatedFeedbackStatus(feedbackState: FeedbackTaskInstance) {   
    console.log("updatedFeedbackStatus function called"); 
    await Api.updateTaskState(feedbackState, TASK_TYPE_IDS.FEEDBACK_TASK, feedbackState.taskInstanceId)
    .then((response) => {
      if (areAllQuestionsAnswered()) {
        props.setCanCompleteTask(true);
      }
      else {
        props.setCanCompleteTask(false);
      }
      void props.fetchTaskInstances();
    })
    .catch((error) => {
      toast("Failed to update feedback task's state");
    })
  }

  function updateResponse(index: number, response: number) {
    const updatedState = {...feedbackState};
    updatedState.responses[index] = response;
    // update state on backend to reflect changes
    void updatedFeedbackStatus(updatedState);
    props.setCanCompleteTask(areAllQuestionsAnswered());   
    setFeedbackState(updatedState);
  }

  useEffect(() => {
    props.setCanCompleteTask(areAllQuestionsAnswered());   
  }, []);
    
  return (
    <div className='flex flex-col gap-3 p-2 overflow-y-auto'>
      {
        props.feedbackTemplate.questions.map((question, questionIndex) => (
          <div key={questionIndex} className='flex flex-col gap-1'>
            <h1 className='font-semibold'>Question {questionIndex + 1}</h1>
            <Separator/>
            <span>{question.question}</span>
            <RadioGroup disabled={props.taskStatus === "complete"} defaultValue="0" className='py-1' 
              value={props.feedbackInstance.responses[questionIndex].toString()} 
              onValueChange={(value: string) => updateResponse(questionIndex, +value)}
            >
              {
                question.likertScale.map((label, labelIndex) => (
                  <div key={labelIndex} className="flex items-center space-x-2">
                    <RadioGroupItem value={labelIndex.toString()} className='cursor-pointer'/>
                    <Label className='cursor-pointer' onClick={() => {updateResponse(questionIndex, labelIndex)}}>{label}</Label>
                  </div>
                ))
              }
            </RadioGroup>
          </div>
        ))
      }
    </div>
  )
}
