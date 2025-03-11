import { Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import { CircleCheckBig } from 'lucide-react';
import React from 'react'

export default function EndNode() {
  return (
    <div className='p-3' style={{backgroundColor: "var(--background)", borderLeft: "10px solid #a83232", borderRadius: "5px", minWidth: "15em",
      boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"
    }}>
      <div className='flex flex-row gap-2 items-center justify-center'>
        <CircleCheckBig/>
        <h1 className='text-sm font-bold'>End of Workflow</h1>
      </div>
      <Handle type="target" position={Position.Top} />
    </div>
  );
}
