import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { NodeProps, Node, Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import { Trash2 } from 'lucide-react';
import React, { useCallback } from 'react'

export type TaskNode = Node<
  {
    index: number;
    taskTitle: number;
    assignee: number;
    deleteTask: (index: number) => void;
  },
  'counter'
>;

export default function TaskNode(props: NodeProps<TaskNode>) {
  const handleStyle = {};

  const onChange = useCallback((evt: { target: { value: any; }; }) => {
    console.log(evt.target.value);
  }, []);
  
  return (
    <div style={{backgroundColor: "black", padding: "6px", border: "1px solid white", borderRadius: "10px", width: "15em"}}>
      <Handle type="target" position={Position.Top} />
      <div style={{display: "flex", flexDirection: "column", gap: "8px"}}>
        <div className='flex flex-row justify-between gap-8 items-center relative' style={{}}>
          <label htmlFor="text" className='text-sm' style={{textAlign: "center", textOverflow: "ellipsis", whiteSpace: "nowrap", width: "15em", overflow: "hidden"}}>{props.data.taskTitle}</label>
          <Button variant="destructive" size="icon" className='rounded-2xl' onClick={() => {props.data.deleteTask(props.data.index);}}><Trash2/></Button>
        </div>
        <Separator/>
        <div className='flex flex-row gap-8'>
          <label htmlFor="text" className='text-xs flex-4'>Description:</label>
          <label htmlFor="text" className='text-xs flex-8'>[Task Description]</label>
        </div>
        <div className='flex flex-row gap-8'>
          <label htmlFor="text" className='text-xs flex-4'>Assignee:</label>
          <label htmlFor="text" className='text-xs flex-8'><Badge>{props.data.assignee}</Badge></label>
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
  );
}
