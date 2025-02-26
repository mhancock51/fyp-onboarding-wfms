import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { addEdge, Background, BackgroundVariant, Controls, MarkerType, Panel, ReactFlow, useEdgesState, useNodesState, Node, Edge } from '@xyflow/react';
import './CreateWorkflowPage.css';
import TaskNode from './Nodes/TaskNode';
import StartNode from './Nodes/StartNode';
import EndNode from './Nodes/EndNode';
import { Button } from '@/components/ui/button';
import WorkflowTemplate from '@/models/WorkflowTemplate';
import WorkflowTask from '@/models/WorkflowTask';
import AddTaskNode from './Nodes/AddTaskNode';
import AddTaskDialog from './AddTaskDialog';

export default function CreateWorkflowPage() {
  const START_NODE_ID = "-1";
  const END_NODE_ID = "-2";
  const ADD_TASK_NODE_ID = "-3";

  const ARROW_MARKER_END = {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' };

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

  const initialNodes: Node[] = [];
  const initialEdges: Edge[] = [];

  const [nodes, setNodes, onNodesChange] = useNodesState(initialNodes);
  const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);

  const [openDialog, setOpenDialog] = useState<boolean>(false);
 
  const onConnect = useCallback(
    (params: any) => setEdges((eds) => addEdge(params, eds)),
    [setEdges],
  );

  const nodeTypes = useMemo(() => ({ customNode: TaskNode, startNode: StartNode, endNode: EndNode, addTaskNode: AddTaskNode }), []);

  function deleteTask(taskIndex: number) {
    setWorkflowTemplate((workflow) => ({ ...workflow, tasks: workflow.tasks.filter((_, index) => index !== taskIndex)}));
  }

  function renderWorkflow() {
    let nodes: any[] = [];
    let edges: any[] = [];
    // add start node
    nodes.push({ id: START_NODE_ID, type: "startNode",  position: { x: 0, y: 0   }});
    // add nodes for each task
    workflowTemplate?.tasks.forEach((task: WorkflowTask, index: number) => {
      const yPos = 75 + (175 * (index));    
      let node = { id: `${index}`, type: "customNode", position: { x: 0, y: yPos }, data: { index: index, taskTitle: `${task.taskId}`, assignee: task.assigneeUserId, deleteTask: () => { deleteTask(index)} }};
      nodes.push(node);
      // add edge to connect to previous node
      const previousNode = nodes[nodes.length - 2];      
      edges.push({ id: `e${previousNode.id}-${index}`, source: previousNode.id, target: `${index}`, markerEnd: ARROW_MARKER_END, style: { strokeWidth: 4, stroke: '#009DD8'} })
    });
    // add "add task" node
    nodes.push({id: ADD_TASK_NODE_ID, type: "addTaskNode", position: { x: 0, y: nodes[nodes.length - 1].position.y + 175 }, data: { onClick: () => {setOpenDialog(true);}}});
    // add connecting node
    let previousNode = nodes[nodes.length - 2]; 
    edges.push({ id: `e${previousNode.id}-${ADD_TASK_NODE_ID}`, source: previousNode.id, target: ADD_TASK_NODE_ID, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} });

    // add end node    
    nodes.push({ id: END_NODE_ID, type: "endNode",  position: { x: 0, y: nodes[nodes.length - 1].position.y + 75 }});
    // add edge to connect to end node
    previousNode = nodes[nodes.length - 2];      
    edges.push({ id: `e${previousNode.id}-${END_NODE_ID}`, source: previousNode.id, target: `${END_NODE_ID}`, markerEnd: {type: MarkerType.ArrowClosed, width: 10, height: 10, color: '#009DD8' }, style: { strokeWidth: 4, stroke: '#009DD8'} })

    setNodes(nodes);
    setEdges(edges);
  }

  function addTask(task: WorkflowTask) {
    setOpenDialog(false);
    setWorkflowTemplate((workflow) => ({ ...workflow, tasks: [...workflow?.tasks, task]}));
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
          defaultViewport={{x: 650, y: 200, zoom: 0.75}}
        >
          <Background variant={BackgroundVariant.Dots} gap={12} size={1} />  
          <Controls />
          {/* <Panel position="top-left" className='bg-white text-black p-6 flex flex-col gap-4 rounded-lg'>                
            Toolbox
            <Button className='bg-blue-400' onClick={addTask}>Add Task to Workflow</Button>                                             
          </Panel> */}
        </ReactFlow>        
      </div>
      <Button style={{float: "right", marginTop: "8px"}} className='cursor-pointer'>Save Workflow</Button>
      <Button style={{float: "right", marginTop: "8px"}} className='cursor-pointer' onClick={() => {setOpenDialog(!openDialog);}}>Open Dialog</Button>
      <AddTaskDialog open={openDialog} onAdd={addTask}/>
    </div>
  )
}
