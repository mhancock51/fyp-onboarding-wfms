import { Badge } from '@/components/ui/badge';
import { Card } from '@/components/ui/card';
import { HoverCard, HoverCardContent, HoverCardTrigger } from '@/components/ui/hover-card';
import { Label } from '@/components/ui/label';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Separator } from '@/components/ui/separator';
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode';
import { NodeProps, Node, Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import { ChevronDown, ChevronUp, Trash2 } from 'lucide-react';
import React, { useCallback, useState } from 'react'
import TaskTypeBadge from '../../MyTasksPage/TaskTypeBadge';

export type TaskNode = Node<
  {    
    taskTitle: string;
    description: string;
    assignee: string;
    taskDependencies: WorkflowTemplateNode[];
    taskTypeId: string;
    deleteTask: () => void;
    moveTaskUp: () => void;
    moveTaskDown: () => void;
    daysUntilDue: number | null;
  }
>;

export default function TaskNode(props: NodeProps<TaskNode>) {
  const handleStyle = {};

  const onChange = useCallback((evt: { target: { value: any; }; }) => {
    console.log(evt.target.value);
  }, []);

  const [openPopover, setOpenPopover] = useState<boolean>(false);
  
  return (
    <Popover open={openPopover} onOpenChange={setOpenPopover}>
      <PopoverTrigger asChild>
        <div className='p-2' style={{color: "var(--foreground)", backgroundColor: "var(--background)", borderRadius: "10px", width: "24em",
          boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"
        }} onClick={() => {setOpenPopover(true)}}>
          <Handle type="target" position={Position.Top} />
          <div className='flex flex-col justify-center gap-1'>
            <div className='flex flex-row justify-center gap-8 text-center py-1 w-full'>
              <h1 className='text-base w-full font-bold'>{props.data.taskTitle}</h1>              
            </div>
            <Separator/>
            <div className='flex flex-row gap-2 py-1 justify-center w-full'>
              <Badge className='rounded-full'>{props.data.assignee}</Badge>
              {
                props.data.taskDependencies.length > 0 &&
                <HoverCard>
                  <HoverCardTrigger>
                    <Badge className='rounded-full'>{props.data.taskDependencies.length} Dependencies</Badge>
                  </HoverCardTrigger> 
                  <HoverCardContent side='top' className='p-1' style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
                    <div className='flex flex-col gap-1 text-sm'>
                      <Label>{props.data.taskDependencies.length > 0 ? "Dependencies" : "Dependency"}</Label>
                      <Separator/>
                    {
                      props.data.taskDependencies.map((dependency, index) => (
                        <Label key={index} className='text-xs cursor-pointer'>{dependency.taskTemplate?.name}</Label>
                      ))
                    }
                    </div>
                  </HoverCardContent>
                </HoverCard>
              }             
              <TaskTypeBadge taskTypeId={props.data.taskTypeId}/>
              {
                props.data.daysUntilDue !== null && props.data.daysUntilDue !== undefined &&
                <Badge className='rounded-full'>{props.data.daysUntilDue} Days</Badge>
              }
            </div>
          </div>
          <Handle type="source" position={Position.Bottom} id="a" />
          <Handle
            type="source"
            position={Position.Bottom}
            id="b"
            style={handleStyle}
          />
        </div>
      </PopoverTrigger>
      <PopoverContent side="right" className='rounded-full w-[40px] p-0 py-2'>
        <div className='flex flex-col gap-2 w-full justify-center items-center'>
          <ChevronUp className='cursor-pointer hover:text-blue-700' onClick={() => {props.data.moveTaskUp(); setOpenPopover(false);}}/>
          <ChevronDown className='cursor-pointer hover:text-blue-700' onClick={() => {props.data.moveTaskDown(); setOpenPopover(false);}}/>
          <Trash2 className='cursor-pointer hover:text-destructive' onClick={() => {props.data.deleteTask(); setOpenPopover(false);}}/>
        </div>        
      </PopoverContent>
    </Popover>
  );
}
