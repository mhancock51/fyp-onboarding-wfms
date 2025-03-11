import { Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import { CircleCheckBig } from 'lucide-react';
import React from 'react'

export default function EndNode() {
  return (
    <div className='p-3' style={{backgroundColor: "#a83232", border: "1px solid white", borderRadius: "25px", minWidth: "15em"}}>
      <div className='flex flex-row gap-2 items-center justify-center'>
        <CircleCheckBig/>
        <h1 className='text-sm font-bold'>End of Workflow</h1>
      </div>
      <Handle type="target" position={Position.Top} />
    </div>
  );
}
