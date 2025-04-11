import { Badge } from '@/components/ui/badge';
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover';
import { Separator } from '@/components/ui/separator';
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode';
import { NodeProps, Node, Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import { ChevronDown, ChevronUp, EyeOff, Plus, Trash2, X, XCircle } from 'lucide-react';
import React, { useCallback, useEffect, useState } from 'react'
import TaskTypeBadge from '../../MyTasksPage/TaskTypeBadge';
import AccountDirectoryBadge from '@/components/AccountDirectoryBadge';

export type TaskNode = Node<
  { 
    // task data  
    node: WorkflowTemplateNode;                 
    // additional methods/metadata    
    deleteTask: () => void;
    canMoveUp: boolean;
    moveTaskUp: () => void;
    canMoveDown: boolean;
    moveTaskDown: () => void;
    isReadOnly: boolean;
    isADependency: boolean;
    removeDependency: () => void;
    addDependency: () => void;
    setSelectedNode: () => void;
    clearSelectedNode: () => void;
    isSelectedNode: boolean;
    selectedNode: WorkflowTemplateNode | null;
    nodes: WorkflowTemplateNode[];
  }
>;

export default function TaskNode(props: NodeProps<TaskNode>) {
  const [openPopover, setOpenPopover] = useState<boolean>(false);  
  
  function handleClick() {
    // toggle highlighting of dependency nodes
    if (openPopover) {
      props.data.clearSelectedNode();
    }
    else {
      props.data.setSelectedNode();
    }
    setOpenPopover(!openPopover);
  }

  function handleOpenChange(open: boolean) {
    // toggle highlighting of dependency nodes
    if (open) {
      props.data.setSelectedNode();
    }    
    setOpenPopover(open);
  }

  function canCreateDependency() {
    // check that a dependency on this node can actually be created
    if (props.data.isADependency) return false;
    if (props.data.isSelectedNode) return false;
    if (props.data.selectedNode === null) return false;
    // ensure that node comes before the selected node
    const currentIndex = props.data.nodes.indexOf(props.data.node);
    const selectedNodeIndex = props.data.nodes.indexOf(props.data.selectedNode);
    if (currentIndex < selectedNodeIndex) {
      return true;
    }
    else {
      return false;
    }
  }

  const BADGE_SIZE = "w-[140px]";

  useEffect(() => {
    if (openPopover && !props.data.isSelectedNode) {
      setOpenPopover(false);
    }
  }, [props.data.isSelectedNode]);

  return (
    <Popover open={openPopover && !props.data.isReadOnly}
     onOpenChange={handleOpenChange}
    >
      <PopoverTrigger asChild>
        <div className={`group relative p-2 rounded-[20px] min-w-[30em] color-foreground bg-background ${props.data.isSelectedNode ? "border-blue-500 border-3" : ""} ${props.data.isADependency ? "border-red-300 border-3" : ""}`} 
          style={{boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}} onClick={(event: any) => {event.stopPropagation(); handleClick();}}
        >
          {
            props.data.isADependency &&
            <span className='absolute top-[-30px] text-red-300'>Dependency</span>
          }
          {
            props.data.isADependency &&
            /// button to remove dependency that selected node has on this
            <div className='invisible group-hover:visible absolute top-[-12px] right-[-12px] bg-background rounded-full hover:bg-gray-200 p-2 cursor-pointer'
              onClick={(event: any) => { event.stopPropagation(); props.data.removeDependency();}}
            >
              <X size={20}/>
            </div>
          }
          {
            canCreateDependency() &&
            /// button to create a dependency on this node by the selected node
            <div className='invisible group-hover:visible absolute top-[-12px] right-[-12px] bg-background rounded-full hover:bg-gray-200 p-2 cursor-pointer'
              onClick={(event: any) => { if (!canCreateDependency()) return; event.stopPropagation(); props.data.addDependency();}}
            >
              <Plus size={20}/>
            </div>
          }
          <Handle type="target" position={Position.Top} />
          {/** Content of the node */}
          <div className='flex flex-col justify-center gap-1'>
            <div className='flex flex-row justify-between px-1 gap-2 text-center w-full'>
              <h1 className='text-base font-bold'>{props.data.node.taskTemplate?.name}</h1>   
              {
                props.data.node.daysUntilDue !== null && props.data.node.daysUntilDue !== undefined &&
                <Badge className={`rounded-full cursor-pointer px-4`}>
                  {props.data.node.daysUntilDue} Days
                </Badge>
              }
            </div>
            <Separator/>
            <div className='m-auto flex flex-row justify-center gap-2 py-1 w-full auto-rows-fr'>
              {
                props.data.node.taskTemplate?.taskTypeId !== undefined &&
                <TaskTypeBadge taskTypeId={props.data.node.taskTemplate?.taskTypeId} className={BADGE_SIZE}/>
              }
              <AccountDirectoryBadge accountDirectory={props.data.node.assignee} className={BADGE_SIZE}/>              
              {
                props.data.node.taskDependencies.length > 0 &&
                <Badge className={`p-2 w-full rounded-full cursor-pointer ${BADGE_SIZE}`}>
                  {props.data.node.taskDependencies.length} Dependencies
                </Badge>
              }                           
            </div>
          </div>
          <Handle type="source" position={Position.Bottom} id="a" />
          <Handle type="source" position={Position.Bottom} id="b" />
        </div>
      </PopoverTrigger>
      <PopoverContent side="right" className='rounded-full w-[40px] p-0 py-2'>
        {
          props.data.isSelectedNode &&
          <div className='flex flex-col gap-2 w-full justify-center items-center'>
            <div className={`cursor-pointer ${!props.data.canMoveUp ? "text-gray-200" : "" } hover:${props.data.canMoveUp ? 'text-blue-700' : ''}`}   onClick={() => { if(!props.data.canMoveUp) return; props.data.moveTaskUp(); setOpenPopover(false);}}>
              <ChevronUp />
            </div>
            <div className={`cursor-pointer ${!props.data.canMoveDown ? "text-gray-200" : "" } hover:${props.data.canMoveDown ? 'text-blue-700' : 'text-gray-200'}`} onClick={() => { if(!props.data.canMoveDown) return; props.data.moveTaskDown(); setOpenPopover(false);}}>
              <ChevronDown />
            </div>
            <div className={`cursor-pointer hover:text-blue-700`} 
              onClick={props.data.clearSelectedNode}
            >
              <EyeOff/>
            </div>
            <div className={'cursor-pointer hover:text-destructive'} onClick={() => {props.data.deleteTask(); setOpenPopover(false);}}>
              <Trash2/>
            </div>
          </div>        
        }        
      </PopoverContent>
    </Popover>
  );
}
