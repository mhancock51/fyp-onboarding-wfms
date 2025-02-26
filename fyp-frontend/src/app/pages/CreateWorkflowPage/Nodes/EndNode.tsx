import { Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import React from 'react'

export default function EndNode() {
  return (
    <div style={{backgroundColor: "#a83232", padding: "5px", border: "1px solid white", borderRadius: "25px", minWidth: "15em"}}>
      <h1 style={{textAlign: "center"}} className='text-sm'>Workflow End</h1>
      <Handle type="target" position={Position.Top} />
    </div>
  );
}
