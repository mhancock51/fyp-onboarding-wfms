import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { NodeProps, Node, Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import React, { useCallback } from 'react'

export type CounterNode = Node<
  {
    taskTitle: number;
    assignee: number;
  },
  'counter'
>;

export default function TestCustomNode(props: NodeProps<CounterNode>) {
  const handleStyle = {};

  const onChange = useCallback((evt: { target: { value: any; }; }) => {
    console.log(evt.target.value);
  }, []);
  
  return (
    <div style={{backgroundColor: "black", padding: "1em", border: "1px solid white", borderRadius: "10px", minWidth: "15em"}}>
      <Handle type="target" position={Position.Top} />
      <div style={{display: "flex", flexDirection: "column", gap: "8px"}}>
        <label htmlFor="text" className='text-sm' style={{textAlign: "center"}}>{props.data.taskTitle}</label>
        <Separator/>
        <label htmlFor="text" className='text-xs'>Description: [Task Description]</label>
        <label htmlFor="text" className='text-xs'>Assignee: <Badge>{props.data.assignee}</Badge></label>        
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
