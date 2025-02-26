import React, { useCallback, useMemo } from 'react'
import { addEdge, Background, BackgroundVariant, Controls, MarkerType, Panel, ReactFlow, useEdgesState, useNodesState } from '@xyflow/react';
import './CreateWorkflowPage.css';
import TestCustomNode from './Nodes/TestCustomNode';
import StartNode from './Nodes/StartNode';
import EndNode from './Nodes/EndNode';
import { Button } from '@/components/ui/button';

export default function CreateWorkflowPage() {
  const endNodeId = "-1";
  const initialNodes = [
    { id: '1', type: "startNode",  position: { x: 0, y: 0   },  data: { label: '1'}},
    { id: '2', type: "customNode", position: { x: 0, y: 75  }, data: { label: '2', taskTitle: "Setup developer environment", assignee: "John Doe" }},
    { id: '3', type: "customNode", position: { x: 0, y: 250 }, data: { label: '3', taskTitle: "Review the architecture and key components of the codebase", assignee: "Jane Smith" }},
    { id: endNodeId, type: "endNode",  position:   { x: 0, y: 425 },  data: { label: '1'}},
  ];
  const initialEdges = [
    { id: 'e1-2', source: '1', target: '2', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} },
    { id: 'e2-3', source: '2', target: '3', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} },
    { id: 'e3-4', source: '3', target: '4', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} }
  ];

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
 
  const onConnect = useCallback(
    (params: any) => setEdges((eds) => addEdge(params, eds)),
    [setEdges],
  );

  function addTaskToWorkflow() {        
    const yPos = 75 + (175 * (nodes.length - 2));
    const index = nodes.length + 1;
    const newNode = { id: `${index}`, type: "customNode", position: { x: 0, y: yPos }, data: { label: `${index}`, taskTitle: `New Task ${index}`, assignee: "Jane Smith" }};    
    setNodes([...nodes, newNode]);
    // create edge
    const previousNodeId = nodes[nodes.length - 2].id;    
    const edgeA: any = { id: `e${previousNodeId}-${index}`, source: previousNodeId, target: `${index}`, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} };
    const edgeB: any = { id: `e${index}-${endNodeId}`, source: `${index}`, target: endNodeId, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} };

    setEdges((edges) => [...edges, ...[ edgeA, edgeB]]); 

    // move end node down
    setNodes((nodes: any[]) =>
      nodes.map((node) =>
        node.id === '-1' ? { ...node, position: { x: 0, y: yPos + 175 } } : node
      )
    );
  }

  function addTwoTasksToWorkflow() {        
    const yPos = 100 + 75 + (175 * (nodes.length - 2));
    const index = nodes.length + 1;
    const newNodeA = { id: `${index}`, type: "customNode", position: { x: -250, y: yPos }, data: { label: `${index}`, taskTitle: `New Task ${index}`, assignee: "Jane Smith" }};    
    const newNodeB = { id: `${index + 1}`, type: "customNode", position: { x: 250, y: yPos }, data: { label: `${index + 1}`, taskTitle: `New Task ${index + 1}`, assignee: "Jane Smith" }};    
    setNodes((nodes) => [...nodes, ...[newNodeA, newNodeB]]);
    // create edge
    const previousNodeId = nodes[nodes.length - 2].id;    
    const edgeA: any = { id: `e${previousNodeId}-${index}`, source: previousNodeId, target: `${index}`, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} };
    const edgeB: any = { id: `e${previousNodeId}-${index + 1}`, source: previousNodeId, target: `${index + 1}`, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} };
    const edgeC: any = { id: `e${index}-${endNodeId}`, source: `${index}`, target: endNodeId, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} };
    const edgeD: any = { id: `e${index + 1}-${endNodeId}`, source: `${index + 1}`, target: endNodeId, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} };

    setEdges((edges) => [...edges, ...[ edgeA, edgeB, edgeC, edgeD]]); 

    // move end node down
    setNodes((nodes: any[]) =>
      nodes.map((node) =>
        node.id === '-1' ? { ...node, position: { x: 0, y: yPos + 200 } } : node
      )
    );
  }

  const nodeTypes = useMemo(() => ({ customNode: TestCustomNode, startNode: StartNode, endNode: EndNode }), []);

  return (
    <div>
      <div>
        <h1>Workflow Name</h1>
        <span>Workflow Description</span>
      </div>
      <div style={{ width: '1500px', height: '700px', margin: "auto"}}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          onNodesChange={onNodesChange}
          // onEdgesChange={onEdgesChange}
          // onConnect={onConnect}
        >
          <Background variant={BackgroundVariant.Dots} gap={12} size={1} />  
          <Controls />
          <Panel position="top-left" className='bg-white text-black p-6 flex flex-col gap-4 rounded-lg'>                
            Toolbox
            <Button className='bg-blue-400' onClick={addTaskToWorkflow}>Add Task to Workflow</Button>            
            <Button className='bg-blue-400' onClick={addTwoTasksToWorkflow}>Add 2 Tasks to Workflow</Button>            
          </Panel>
        </ReactFlow>        
      </div>
      <Button style={{float: "right", marginTop: "8px"}} className='cursor-pointer'>Save Workflow</Button>
    </div>
  )
}
