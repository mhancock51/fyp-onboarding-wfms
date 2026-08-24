import Api from '@/api';
import NoResults from '@/components/NoResults';
import { ChartConfig, ChartContainer, ChartTooltip, ChartTooltipContent } from '@/components/ui/chart';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Spinner } from '@/components/ui/spinner';
import WorkflowTemplateDTO from '@/models/DTOs/WorkflowTemplateDTO';
import HTTPresponse from '@/models/HTTPresponse';
import { FeedbackTaskInstance } from '@/models/tasks/FeedbackTaskInstance';
import { FeedbackTaskTemplate, LikertQuestion } from '@/models/tasks/FeedbackTaskTemplate';
import TaskInstanceDTO from '@/models/tasks/TaskInstanceDTO';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import { AxiosResponse } from 'axios';
import React, { useEffect, useState } from 'react'
import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from 'recharts';
import { toast } from 'sonner';

interface Props {
  open: boolean;
  setOpen: React.Dispatch<React.SetStateAction<boolean>>;
  workflowTemplate: WorkflowTemplateDTO;
}

export default function WorkflowTemplateFeedbackDialog(props: Props) {
  const [feedbackTasks, setFeedbackTasks] = useState<TaskInstanceDTO[]>([]);
  const [uniqueFeedbackTemplates, setUnqiueFeedbackTemplates] = useState<TaskTemplate[]>([]);
  const [loading, setLoading] = useState(false);

  async function fetchWorkflowTemplateFeedback() {
    setLoading(true);
    await Api.feedback.fetchWorkflowTemplateFeedback(props.workflowTemplate.id)
    .then((response: AxiosResponse<HTTPresponse<TaskInstanceDTO[], string>>) => {
      const feedbackTaskInstances = response.data.data
      setFeedbackTasks(feedbackTaskInstances);
      // find the unique feedback task templates from the list
      setUnqiueFeedbackTemplates(getUniqueTaskTemplatesFromInstances(feedbackTaskInstances));
    })
    .catch((error) => {
      if (error.response.data.error) {
        toast.error(error.response.data.error);
      }
      else {
        toast.error("Failed to retrieve feedback for workflow template");
      } 
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function getUniqueTaskTemplatesFromInstances(taskInstances: TaskInstanceDTO[]): TaskTemplate[] {
    var uniqueTemplates: TaskTemplate[] = [];
    taskInstances.forEach(taskInstances => {
      if (!uniqueTemplates.map(t => t.id).includes(taskInstances.template.id)) {
        // template is unique and not in the list
        uniqueTemplates.push(taskInstances.template);
      }
    });
    return uniqueTemplates;
  }

  function closeAndClear() {
    props.setOpen(false);
    setUnqiueFeedbackTemplates([]);
    setFeedbackTasks([]);
  }

  useEffect(() => {
    void fetchWorkflowTemplateFeedback();
  }, [props.workflowTemplate]);

  useEffect(() => {
    void fetchWorkflowTemplateFeedback();
  }, []);

  return (
    <Dialog open={props.open} onOpenChange={closeAndClear}>
      <DialogContent className='min-w-[700px]'>
        <DialogHeader>
          <DialogTitle>Workflow Template Feedback</DialogTitle>          
        </DialogHeader>
        <div className="grid gap-4 py-4 max-h-[70vh] overflow-y-auto">
          {
            loading &&
            <div className='flex flex-row w-full gap-4'>
              <Spinner/>
              Loading workflow template feedback...
            </div>
          }
          {
            !loading && feedbackTasks.length === 0 &&
            <NoResults text={'No complete feedback tasks for this workflow template'}/>
          }
          {
            feedbackTasks.length > 0 &&
            <div>
              {
                uniqueFeedbackTemplates.map((feedbackTemplate, index) => (
                  <div key={index} className='felx flex-col gap-2 w-full'>
                    <h1 className='text-md font-semibold'>{feedbackTemplate.name} ({feedbackTasks.filter(i => i.template.id === feedbackTemplate.id).length} responses)</h1>
                    <div className='flex flex-col gap-2 w-full'>
                      {
                        /* Display all questions and the agragated responses */
                        (feedbackTemplate.taskTypeData as FeedbackTaskTemplate).questions.map((question, index) => (
                          <LikertAggregateResponseBarChart 
                            question={question} 
                            index={index} 
                            feedbackTemplate={feedbackTemplate} 
                            feedbackTasks={feedbackTasks}/>
                        ))
                      }
                    </div>

                  </div>
                ))
              }
            </div>
          }
        </div>
      </DialogContent>      
    </Dialog>
  )
}

interface BarChartProps {
  question: LikertQuestion;
  index: number;
  feedbackTemplate: TaskTemplate;
  feedbackTasks: TaskInstanceDTO[];
}

function LikertAggregateResponseBarChart(props: BarChartProps) {
  const RESPONSES_CHART_CONFIG = {
    total: {
      label: "Responses",
      color: "hsl(var(--primary))",
    }
  } satisfies ChartConfig;

  function getNumberOfResponsesByLikertLabel(questionIndex: number, likertIndex: number, feedbackTaskTemplate: TaskTemplate) {
    // find all task instances matching task template
    var relevantFeedbackTasks = props.feedbackTasks.filter(i => i.template.id === feedbackTaskTemplate.id);
    var total = relevantFeedbackTasks.filter(i => (i.instanceData as FeedbackTaskInstance).responses[questionIndex] === likertIndex).length;
    console.log("total: ", total);
    return total;
  }
  
  return (
    <div key={props.index}>
      <h2>Question {props.index + 1}: {props.question.question}</h2>
      <ChartContainer config={RESPONSES_CHART_CONFIG} className="w-[600px] h-[250px]">
        <BarChart accessibilityLayer data={props.question.likertScale.map((label, labelIndex) => (
          { likertLabel: label, total: getNumberOfResponsesByLikertLabel(props.index, labelIndex, props.feedbackTemplate)}                                
        ))}>
          <CartesianGrid vertical={false} />
          <XAxis
            dataKey="likertLabel"
            tickLine={false}
            tickMargin={10}
            axisLine={false}                                  
          />
          <YAxis domain={[0, 10]} />
          <ChartTooltip content={<ChartTooltipContent />} />
          <Bar dataKey="total" fill="var(--color-total)" radius={2} />
        </BarChart>
      </ChartContainer>                  
    </div>
  )
}
