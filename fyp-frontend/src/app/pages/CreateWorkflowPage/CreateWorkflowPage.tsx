import React, { useCallback, useMemo } from 'react'
import { addEdge, Background, BackgroundVariant, Controls, MarkerType, ReactFlow, useEdgesState, useNodesState } from '@xyflow/react';
import './CreateWorkflowPage.css';
import TestCustomNode from './Nodes/TestCustomNode';
import StartNode from './Nodes/StartNode';

export default function CreateWorkflowPage() {
  const initialNodes = [
    { id: '1', type: "startNode",  position: { x: 0, y: 0   },  data: { label: '1'}},
    { id: '2', type: "customNode", position: { x: 0, y: 100   }, data: { label: '2', taskTitle: "Task 1", assignee: "John Doe" }},
    { id: '3', type: "customNode", position: { x: 0, y: 300 }, data: { label: '3', taskTitle: "Task 2", assignee: "Jane Smith" }},
  ];
  const initialEdges = [
    { id: 'e1-2', source: '1', target: '2', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#FF0072' }, style: { strokeWidth: 4, stroke: '#FF0072'} },
    { id: 'e2-3', source: '2', target: '3', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#FF0072' }, style: { strokeWidth: 4, stroke: '#FF0072'} }
  ];

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
 
  const onConnect = useCallback(
    (params: any) => setEdges((eds) => addEdge(params, eds)),
    [setEdges],
  );

  const nodeTypes = useMemo(() => ({ customNode: TestCustomNode, startNode: StartNode }), []);

  return (
    <div>
      <h1>Create a workflow</h1>
      <div style={{ width: '1500px', height: '700px', margin: "auto"}}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          // onNodesChange={onNodesChange}
          // onEdgesChange={onEdgesChange}
          // onConnect={onConnect}
        >
          <Controls />
          <Background variant={BackgroundVariant.Dots} gap={12} size={1} />  
        </ReactFlow>        
      </div>
    </div>
  )
}
