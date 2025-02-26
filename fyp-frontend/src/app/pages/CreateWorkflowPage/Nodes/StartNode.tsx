import { Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import React from 'react'

export default function StartNode() {
  const handleStyle = {};

  return (
    <div style={{backgroundColor: "#199c49", padding: "5px", border: "1px solid white", borderRadius: "25px", minWidth: "15em"}}>
      <h1 style={{textAlign: "center"}} className='text-sm'>Start of Workflow</h1>
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
