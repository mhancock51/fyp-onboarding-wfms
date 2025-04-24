import Api from '@/api';
import NoResults from '@/components/NoResults';
import { Badge } from '@/components/ui/badge';
import { Card } from '@/components/ui/card';
import { Checkbox } from '@/components/ui/checkbox';
import { Separator } from '@/components/ui/separator';
import { Tabs, TabsContent, TabsTrigger } from '@/components/ui/tabs';
import { TASK_TYPE_IDS } from '@/constants';
import ProjectObjective from '@/models/tasks/ProjectObjective';
import ProjectTaskInstance from '@/models/tasks/ProjectTaskInstance';
import ProjectTaskTemplate from '@/models/tasks/ProjectTaskTemplate';
import { CheckedState } from '@radix-ui/react-checkbox';
import { Label } from '@radix-ui/react-dropdown-menu';
import { TabsList } from '@radix-ui/react-tabs';
import { Asterisk, ExternalLink } from 'lucide-react';
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
    await Api.updateTaskState(projectInstance, TASK_TYPE_IDS.PROJECT_TASK, props.taskInstanceId)
    .then(() => {
      if (areAllRequiredObjectivesComplete(projectInstance)) {
        props.setCanCompleteTask(true);
      }
      else {
        props.setCanCompleteTask(false);
      }
      void props.fetchTaskInstances();
    })
    .catch(() => {
      toast("Failed to update checklist task's state");
    })
  }

  function updateObjectiveState(complete: boolean, objective: ProjectObjective) {
    if (props.taskStatus !== "open") return;
    // update checklist item's state through hook
    setProjectState((prevState) => ({ 
      ...prevState, 
      // find item by index and set its value
      objectiveStates: prevState.objectiveStates.map((status, i) => (props.projectTemplate.objectives[i] === objective ? complete : status))
    }));
  }

  useEffect(() => {
    setProjectState(props.projectInstance);
  }, [props.projectInstance]);

  useEffect(() => {
    // exit if project state isn't actually set
    if (projectState.id === "") return;
    if (projectState.objectiveStates.length === 0) return;
    if (projectState.objectiveStates === props.projectInstance.objectiveStates) return;
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
            <Label className='font-normal'>
            { props.projectTemplate.deliverable === "" ? "N/A" : props.projectTemplate.deliverable }
            </Label>            
          </div>
          <Separator/>
          <div className='flex flex-col'>
            <Label className='font-bold'>Objectives</Label>
            <div className='flex flex-col gap-1 w-full mx-2 my-1'>
            {
              props.projectTemplate.objectives.filter(o => o.required).map((objective, index) => (
                <div key={index} className='flex flex-row gap-4 items-center'>
                  <Checkbox disabled={props.taskStatus !== "open"} className='data-[state=checked]:bg-green-500' 
                    checked={projectState.objectiveStates[props.projectTemplate.objectives.findIndex(i => i === objective)]} 
                    onCheckedChange={(checked: CheckedState) => {updateObjectiveState(checked as boolean, objective)}}
                  />
                  <div className='flex flex-row w-full justify-between'>
                    <Label>{objective.objective}</Label>
                    {
                      objective.required &&
                      <Asterisk size={20} className='text-red-500'/>
                    }
                  </div>
                </div>
              ))  
            }
            </div>
            <Separator/>
            <div className='flex flex-col gap-1 w-full mx-2 my-1'>
            <Label className='font-bold font-medium'>Optional</Label>
            {
              props.projectTemplate.objectives.filter(o => !o.required).map((objective, index) => (
                <div key={index} className='flex flex-row gap-4 items-center'>
                  <Checkbox disabled={props.taskStatus !== "open"} className='data-[state=checked]:bg-green-500' 
                    checked={projectState.objectiveStates[props.projectTemplate.objectives.findIndex(i => i === objective)]} 
                    onCheckedChange={(checked: CheckedState) => {updateObjectiveState(checked as boolean, objective)}}
                  />
                  <div className='flex flex-row w-full justify-between'>
                    <Label>{objective.objective}</Label>
                  </div>

                </div>
              ))  
            }
            </div>
          </div>
        </div>
      </TabsContent>
      <TabsContent value="support">
        <div className='flex flex-col gap-2 p-2'>
          {
            props.projectTemplate.supportLinks.length !== 0 &&
            <Label className='font-normal'>The following links may help you with this project</Label>
          }
          <div className='flex flex-col gap-2 w-full max-h-[33vh] overflow-y-auto'>
            {
              props.projectTemplate.supportLinks.length === 0 &&
              <NoResults text={'No support links provided for this project'}/>
            }
            {
              props.projectTemplate.supportLinks.map((link, index) => (
                <Card key={index} className='flex flex-col gap-2 w-full my-1 p-2'>
                  <Label className='font-normal'>{link.description}</Label>
                  <div className='flex flex-row justify-center w-full'>
                    <Badge className='p-2 rounded-full cursor-pointer min-w-[250px]' onClick={() => {window.open(link.link, '_blank');}}>
                      {link.linkLabel} <ExternalLink size={50}/>
                    </Badge>                        
                  </div>
                </Card>

              ))
            }
          </div>
        </div>

      </TabsContent>
    </Tabs>
  )
}
