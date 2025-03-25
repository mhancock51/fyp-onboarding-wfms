import { Handle, Position, NodeProps, Node } from '@xyflow/react'
import { CirclePlus } from 'lucide-react';
import React from 'react'

export type AddTaskNode = Node<
  {
    onClick: () => void;    
  }  
>;

export default function AddTaskNode(props: NodeProps<AddTaskNode>) {
  const handleStyle = {};

  return (
    <div className='bg-blue-500 cursor-pointer p-2 hover:bg-blue-400' style={{ border: "1px solid white", borderRadius: "25px", minWidth: "15em"}}>
      <Handle type="target" position={Position.Top} />
      <div className='flex flex-row gap-2 justify-center items-center text-primary-foreground' onClick={props.data.onClick}>
        <CirclePlus/>
        <h1 className='text-sm font-bold'> Add Task</h1>
      </div>
      <Handle type="source" position={Position.Bottom} id="a" />
      <Handle
        type="source"
        position={Position.Bottom}
        id="b"
        style={handleStyle}
      />
    </div>
  )
}
