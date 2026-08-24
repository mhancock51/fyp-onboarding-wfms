import { Handle } from '@xyflow/react';
import { Position } from '@xyflow/system';
import { Play } from 'lucide-react';

export default function StartNode() {
  const handleStyle = {};

  return (
    <div className='p-3 border-l-8 border-l-green-700 bg-background' style={{borderRadius: "5px", minWidth: "15em", boxShadow: "rgba(100, 100, 111, 0.2) 0px 7px 29px 0px"}}>
      <div className='flex flex-row gap-2 items-center justify-center text-green-700'>
        <Play/>
        <h1 className='text-sm font-bold'>Start of Workflow</h1>
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
