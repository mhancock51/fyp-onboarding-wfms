import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance';
import { FeedbackTaskTemplate } from '@/models/tasks/FeedbackTaskTemplate';
import React, { useEffect, useState } from 'react'

interface Props {
  taskInstanceId: string;
  feedbackInstance: FeedbackTaskInstance;
  feedbackTemplate: FeedbackTaskTemplate;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
}

export default function FeedbackTask(props: Props) {
  const DEFAULT_STATE: FeedbackTaskInstance= {
    id: '',
    taskInstanceId: '',
    responses: []
  }

  const [feedbackState, setFeedbackState] = useState<FeedbackTaskInstance>(DEFAULT_STATE);

  useEffect(() => {
    setFeedbackState(props.feedbackInstance);
  }, [props.feedbackInstance]);
    
  return (
    <div className='flex flex-col gap-2 p-2'>
      {
        props.feedbackTemplate.questions.map((question, index) => (
          <div key={index}>
            {question.question}
          </div>
        ))
      }
    </div>
  )
}
