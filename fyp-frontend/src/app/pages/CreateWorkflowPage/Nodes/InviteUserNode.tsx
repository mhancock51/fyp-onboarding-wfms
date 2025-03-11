import { Handle, Position } from '@xyflow/react'
import { Mail } from 'lucide-react'
import React from 'react'

export default function InviteUserNode() {
  return (
    <div className='p-3' style={{backgroundColor: "var(--background)", borderLeft: "10px solid oklch(0.546 0.245 262.881)", borderRadius: "5px", minWidth: "15em",
      boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"
    }}>
      <Handle type="target" position={Position.Top} />
      <div className='flex flex-row gap-2 items-center justify-center' style={{color: "oklch(0.546 0.245 262.881)"}}>
        <Mail/>
        <h1 className='text-sm font-bold'>Onboarder Invited</h1>
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
