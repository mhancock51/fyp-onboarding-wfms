import Api from '@/api';
import { Badge } from '@/components/ui/badge';
import { Card } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsTrigger } from '@/components/ui/tabs';
import ProjectTaskInstance from '@/models/tasks/ProjectTaskInstance';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';
import { CheckedState } from '@radix-ui/react-checkbox';
import { Label } from '@radix-ui/react-dropdown-menu';
import { TabsList } from '@radix-ui/react-tabs';
import { ExternalLink } from 'lucide-react';
import React, { useEffect, useState } from 'react'
import { toast } from 'sonner';

interface Props {
  taskInstanceId: string;
  projectTemplate: ProjectTaskTemplate;
  projectInstance: ProjectTaskInstance;
  fetchTaskInstances: () => Promise<void>;
  setCanCompleteTask: React.Dispatch<React.SetStateAction<boolean>>;
  taskStatus: string;
}
export default function ProjectTask(props: Props) {
  const DEFAULT_STATE: ProjectTaskInstance = {
    id: '',
    taskInstanceId: '',
    objectiveStates: []
  }
  const [projectState, setProjectState] = useState<ProjectTaskInstance>(DEFAULT_STATE);

  function areAllRequiredObjectivesComplete(projectState: ProjectTaskInstance) {
    // ensure all required objectives are complete
    var missedObjective = false;
    projectState.objectiveStates.forEach((objective, index) => {
      if (!objective && props.projectTemplate.objectives[index].required) {
        missedObjective = true;
      }      
    });
    return !missedObjective;
  }

  async function updateTaskInstanceState(projectInstance: ProjectTaskInstance) {
    await Api.updateTaskState(projectInstance, "project-task", props.taskInstanceId)
    .then((response) => {
      toast.success("Successfully updated checklist task's state");
      if (areAllRequiredObjectivesComplete(projectInstance)) {
        props.setCanCompleteTask(true);
      }
      else {
        props.setCanCompleteTask(false);
      }
      void props.fetchTaskInstances();
    })
    .catch((error) => {
      toast("Failed to update checklist task's state");
    })
  }

  function updateObjectiveState(complete: boolean, index: number) {
    if (props.taskStatus !== "open") return;
    // update checklist item's state through hook
    setProjectState((prevState) => ({ 
      ...prevState, 
      // find item by index and set its value
      objectiveStates: prevState.objectiveStates.map((status, i) => (i === index ? complete : status))
    }));
  }

  useEffect(() => {
    setProjectState(props.projectInstance);
  }, [props.projectInstance]);

  useEffect(() => {
    // exit if project state isn't actually set
    if (projectState.id === "") return;
    if (projectState.objectiveStates.length === 0) return;
    // update "can complete" flag if all required objectives complete
    props.setCanCompleteTask(areAllRequiredObjectivesComplete(projectState));
    // update state server side
    void updateTaskInstanceState(projectState);
  }, [projectState.objectiveStates]);

  return (
    <Tabs defaultValue="brief" className="w-full">
      <TabsList className="grid w-full grid-cols-2">
        <TabsTrigger value="brief">Project Brief</TabsTrigger>
        <TabsTrigger value="support">Support</TabsTrigger>
      </TabsList>
      <TabsContent value="brief">
        <div className='flex flex-col gap-2 p-2'>
          <div className='flex flex-col'>
            <Label className='font-bold'>Project Brief</Label>
            <Label className='font-normal'>{ props.projectTemplate.brief }</Label>
          </div>
          <div className='flex flex-col'>
            <Label className='font-bold'>Deliverable</Label>
            <Badge className='mx-2 my-1 py-2 px-4 rounded-full'>A really cool react web app</Badge>            
          </div>
          <Separator/>
          <div className='flex flex-col'>
            <Label className='font-bold'>Objectives</Label>
            <div className='flex flex-col gap-1 w-full mx-2 my-1'>
            {
              props.projectTemplate.objectives.map((objective, index) => (
                <div key={index} className='flex flex-row gap-4 items-center'>
                  <Checkbox className='data-[state=checked]:bg-green-500' checked={projectState.objectiveStates[index]} onCheckedChange={(checked: CheckedState) => {updateObjectiveState(checked as boolean, index)}}/>
                  <Label>{objective.objective} {objective.required ? "" : "(optional)"}</Label>
                </div>
              ))  
            }
            </div>
          </div>
          <div className='flex flex-col w-full gap-2'>
            <Label className='font-bold'>Upload project file</Label>
            <Input type='file' accept='.zip'/>
          </div>
        </div>
      </TabsContent>
      <TabsContent value="support">
        <div className='flex flex-col gap-2 p-2'>
          <Label className='font-bold'>Supporting Links</Label>
          <div className='flex flex-col gap-2 w-full max-h-[33vh] overflow-y-scroll'>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>React Dev Learn webpage - a good start for anyone that hasn't worked with react before</Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://react.dev/learn", '_blank');}}>
                  React Dev Learn <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>This course covers React fundamentals, including JSX, components, state, props, and hooks, through hands-on projects.</Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://www.codecademy.com/learn/react-101", '_blank');}}>
                  Codecademy React 101 <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>A comprehensive YouTube tutorial that guides you through building React applications from scratch. </Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://www.youtube.com/watch?v=BQHRu-lB3SE&ab_channel=WowLearns", '_blank');}}>
                  Advanced React FULL COURSE <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>React Dev Learn webpage - a good start for anyone that hasn't worked with react before</Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://react.dev/learn", '_blank');}}>
                  React Dev Learn <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>This course covers React fundamentals, including JSX, components, state, props, and hooks, through hands-on projects.</Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://www.codecademy.com/learn/react-101", '_blank');}}>
                  Codecademy React 101 <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>A comprehensive YouTube tutorial that guides you through building React applications from scratch. </Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://www.youtube.com/watch?v=BQHRu-lB3SE&ab_channel=WowLearns", '_blank');}}>
                  Advanced React FULL COURSE <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>React Dev Learn webpage - a good start for anyone that hasn't worked with react before</Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://react.dev/learn", '_blank');}}>
                  React Dev Learn <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>This course covers React fundamentals, including JSX, components, state, props, and hooks, through hands-on projects.</Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://www.codecademy.com/learn/react-101", '_blank');}}>
                  Codecademy React 101 <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
            <Card className='flex flex-col gap-2 w-full my-1 p-2'>
              <Label className='font-normal'>A comprehensive YouTube tutorial that guides you through building React applications from scratch. </Label>
              <div className='flex flex-row justify-center w-full'>
                <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open("https://www.youtube.com/watch?v=BQHRu-lB3SE&ab_channel=WowLearns", '_blank');}}>
                  Advanced React FULL COURSE <ExternalLink size={50}/>
                </Badge>                        
              </div>
            </Card>
          </div>
        </div>

      </TabsContent>
    </Tabs>
  )
}
