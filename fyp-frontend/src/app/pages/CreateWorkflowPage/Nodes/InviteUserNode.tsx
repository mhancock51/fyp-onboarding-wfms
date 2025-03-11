import { Handle, Position } from '@xyflow/react'
import { Mail } from 'lucide-react'
import React from 'react'

export default function InviteUserNode() {
  return (
    <div className='p-3' style={{backgroundColor: "#199c49", border: "1px solid white", borderRadius: "25px", minWidth: "15em"}}>
      <Handle type="target" position={Position.Top} />
      <div className='flex flex-row gap-2 items-center justify-center'>
        <Mail/>
        <h1 className='text-sm font-bold text-foreground'>Jeff Invited</h1>
      </div>
      <Handle type="source" position={Position.Bottom} id="a" />
      <Handle
        type="source"
        position={Position.Bottom}
        id="b"        
      />
    </div>
  )
}
