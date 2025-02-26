import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { addEdge, Background, BackgroundVariant, Controls, MarkerType, Panel, ReactFlow, useEdgesState, useNodesState, Node, Edge } from '@xyflow/react';
import './CreateWorkflowPage.css';
import TestCustomNode from './Nodes/TestCustomNode';
import StartNode from './Nodes/StartNode';
import EndNode from './Nodes/EndNode';
import { Button } from '@/components/ui/button';
import WorkflowTemplate from '@/models/WorkflowTemplate';
import WorkflowTask from '@/models/WorkflowTask';

export default function CreateWorkflowPage() {
  const START_NODE_ID = "-1";
  const END_NODE_ID = "-2";

  const DEFAULT_WORKFLOW_TEMPLATE: WorkflowTemplate = {
    name: 'My Workflow',
    description: 'A really cool onboarding workflow',
    tasks: [
      {
        taskId: 'reusable_task_1',
        essential: false,
        assigneeUserId: 'john_doe'
      },
      {
        taskId: 'reusable_task_2',
        essential: false,
        assigneeUserId: 'jane_doe'
      }
    ]
  }
  const [workflowTemplate, setWorkflowTemplate] = useState<WorkflowTemplate>(DEFAULT_WORKFLOW_TEMPLATE);

  // const initialNodes = [
  //   { id: '1', type: "startNode",  position: { x: 0, y: 0   },  data: { label: '1'}},
  //   { id: '2', type: "customNode", position: { x: 0, y: 75  }, data: { label: '2', taskTitle: "Setup developer environment", assignee: "John Doe" }},
  //   { id: '3', type: "customNode", position: { x: 0, y: 250 }, data: { label: '3', taskTitle: "Review the architecture and key components of the codebase", assignee: "Jane Smith" }},
  //   { id: END_NODE_ID, type: "endNode",  position:   { x: 0, y: 425 },  data: { label: '1'}},
  // ];
  // const initialEdges = [
  //   { id: 'e1-2', source: '1', target: '2', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} },
  //   { id: 'e2-3', source: '2', target: '3', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} },
  //   { id: 'e3-4', source: '3', target: '4', markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} }
  // ];
  const initialNodes: Node[] = [];
  const initialEdges: Edge[] = [];

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);
 
  const onConnect = useCallback(
    (params: any) => setEdges((eds) => addEdge(params, eds)),
    [setEdges],
  );

  const nodeTypes = useMemo(() => ({ customNode: TestCustomNode, startNode: StartNode, endNode: EndNode }), []);

  function renderWorkflow() {
    let nodes: any[] = [];
    let edges: any[] = [];
    // add start node
    nodes.push({ id: START_NODE_ID, type: "startNode",  position: { x: 0, y: 0   }});
    // add nodes for each task
    workflowTemplate?.tasks.forEach((task: WorkflowTask, index: number) => {
      const yPos = 75 + (175 * (index));    
      let node = { id: `${index}`, type: "customNode", position: { x: 0, y: yPos }, data: { taskTitle: `New Task ${index} (${task.taskId})`, assignee: task.assigneeUserId }};
      nodes.push(node);
      // add edge to connect to previous node
      const previousNode = nodes[nodes.length - 2];      
      edges.push({ id: `e${previousNode.id}-${index}`, source: previousNode.id, target: `${index}`, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} })
    });
    // add end node
    // add start node
    nodes.push({ id: END_NODE_ID, type: "endNode",  position: { x: 0, y: nodes[nodes.length - 1].position.y + 150 }});


    setNodes(nodes);
    setEdges(edges);
  }

  function addTask() {
    const newTask: WorkflowTask = {
      taskId: `reusable_task_${workflowTemplate?.tasks.length}`,
      essential: false,
      assigneeUserId: "jane_doe"
    }
    setWorkflowTemplate((workflow) => ({ ...workflow, tasks: [...workflow?.tasks, newTask]}));
  }

  useEffect(() => {    
    renderWorkflow();
  }, [workflowTemplate]);

  return (
    <div>
      <div className='m-4'>
        <h1 className='text-lg bold'>{workflowTemplate?.name}</h1>
        <span>{workflowTemplate?.description}</span>
      </div>
      <div style={{ width: '1500px', height: '700px', margin: "auto"}}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          // onNodesChange={onNodesChange}
          // onEdgesChange={onEdgesChange}
          // onConnect={onConnect}
        >
          <Background variant={BackgroundVariant.Dots} gap={12} size={1} />  
          <Controls />
          <Panel position="top-left" className='bg-white text-black p-6 flex flex-col gap-4 rounded-lg'>                
            Toolbox
            <Button className='bg-blue-400' onClick={addTask}>Add Task to Workflow</Button>                                             
          </Panel>
        </ReactFlow>        
      </div>
      <Button style={{float: "right", marginTop: "8px"}} className='cursor-pointer'>Save Workflow</Button>
    </div>
  )
}
