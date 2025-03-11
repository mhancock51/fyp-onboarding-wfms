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
    taskTitle: string;
    description: string;
    assignee: string;
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
    <div className='p-2' style={{color: "var(--foreground)", backgroundColor: "var(--background)", border: "1px solid var(--foreground)", borderRadius: "10px", width: "15em"}}>
      <Handle type="target" position={Position.Top} />
      <div className='flex flex-col justify-start'>
        <div className='flex flex-row justify-between gap-8 items-center w-full'>
          <label htmlFor="text" className='text-sm w-full' style={{textOverflow: "ellipsis", whiteSpace: "nowrap", overflow: "hidden"}}>{props.data.taskTitle}</label>
          <Button variant="destructive" size="icon" className='rounded-2xl min-w-4 h-6 m-1' onClick={() => {props.data.deleteTask(props.data.index);}}><Trash2/></Button>

        </div>
        <Separator/>
        <div className='flex flex-col gap-2'>
          {/* <div className='flex flex-row gap-8'>
            <label htmlFor="text" className='text-xs flex-4'>Description:</label>
            <label htmlFor="text" className='text-xs flex-8' style={{overflowY: "hidden", textOverflow: "ellipsis", height: "2em"}}>{props.data.description}</label>
          </div> */}
          <div className='flex flex-row gap-8 py-2'>
            <label htmlFor="text" className='text-xs flex-4'>Assignee:</label>
            <label htmlFor="text" className='text-xs flex-8'><Badge>{props.data.assignee}</Badge></label>
          </div>             
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
