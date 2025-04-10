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
import React, { useCallback, useEffect, useState } from 'react'
import TaskTypeBadge from '../../MyTasksPage/TaskTypeBadge';
import AccountDirectoryBadge from '@/components/AccountDirectoryBadge';
import AccountDirectory from '@/models/AccountDirectory';

export type TaskNode = Node<
  {    
    taskTitle: string;
    description: string;
    assignee: AccountDirectory;
    taskDependencies: WorkflowTemplateNode[];
    taskTypeId: string;
    deleteTask: () => void;
    canMoveUp: boolean;
    moveTaskUp: () => void;
    canMoveDown: boolean;
    moveTaskDown: () => void;
    daysUntilDue: number | null;
    isReadOnly: boolean;
    isADependency: boolean;
    setDependencyNodes: () => void;
    clearDependencyNodes: () => void;
  }
>;

export default function TaskNode(props: NodeProps<TaskNode>) {
  const [openPopover, setOpenPopover] = useState<boolean>(false);  
  
  function handleClick() {
    // toggle highlighting of dependency nodes
    if (openPopover) {
      props.data.clearDependencyNodes();
    }
    else {
      props.data.setDependencyNodes();
    }
    setOpenPopover(!openPopover);
  }

  function handleOpenChange(open: boolean) {
    // toggle highlighting of dependency nodes
    if (open) {
      props.data.setDependencyNodes();
    }
    else {
      props.data.clearDependencyNodes();
    }
    setOpenPopover(open);
  }

  return (
    <Popover open={openPopover && !props.data.isReadOnly} onOpenChange={handleOpenChange}>
      <PopoverTrigger asChild>
        <div className={`relative p-2 rounded-[20px] min-w-[30em] color-foreground bg-background ${openPopover ? "border-blue-500 border-3" : ""} ${props.data.isADependency ? "border-red-300 border-3" : ""}`} 
          style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}} onClick={handleClick}
        >
          {
            props.data.isADependency &&
            <span className='absolute top-[-30px] text-red-300'>Dependency</span>
          }
          <Handle type="target" position={Position.Top} />
          <div className='flex flex-col justify-center gap-1'>
            <div className='flex flex-row justify-center gap-8 text-center w-full'>
              <h1 className='text-base w-full font-bold'>{props.data.taskTitle}</h1>              
            </div>
            <Separator/>
            <div className='flex flex-row gap-2 py-1 justify-center w-full items-center'>
              <AccountDirectoryBadge accountDirectory={props.data.assignee}/>              
              {
                props.data.taskDependencies.length > 0 &&
                <HoverCard>
                  <HoverCardTrigger>
                    <Badge className='rounded-full py-2 px-4'>{props.data.taskDependencies.length} Dependencies</Badge>
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
                <Badge className='rounded-full py-2 px-4'>{props.data.daysUntilDue} Days</Badge>
              }
            </div>
          </div>
          <Handle type="source" position={Position.Bottom} id="a" />
          <Handle type="source" position={Position.Bottom} id="b" />
        </div>
      </PopoverTrigger>
      <PopoverContent side="right" className='rounded-full w-[40px] p-0 py-2'>
        <div className='flex flex-col gap-2 w-full justify-center items-center'>
          <ChevronUp className={`cursor-pointer hover:${props.data.canMoveUp ? 'text-blue-700' : 'text-gray-200'}`}   onClick={() => { if(!props.data.canMoveUp) return; props.data.moveTaskUp(); setOpenPopover(false);}}/>
          <ChevronDown className={`cursor-pointer hover:${props.data.canMoveDown ? 'text-blue-700' : 'text-gray-200'}`} onClick={() => { if(!props.data.canMoveDown) return; props.data.moveTaskDown(); setOpenPopover(false);}}/>
          <Trash2 className='cursor-pointer hover:text-destructive' onClick={() => {props.data.deleteTask(); setOpenPopover(false);}}/>
        </div>        
      </PopoverContent>
    </Popover>
  );
}
