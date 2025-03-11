import { Background, BackgroundVariant, Controls, Edge, MarkerType, Node, ReactFlow, useEdgesState, useNodesState } from '@xyflow/react'
import React, { useEffect, useMemo, useState } from 'react'
import AddTaskNode from './Nodes/AddTaskNode';
import EndNode from './Nodes/EndNode';
import InviteUserNode from './Nodes/InviteUserNode';
import StartNode from './Nodes/StartNode';
import TaskNode from './Nodes/TaskNode';
import WorkflowTemplateNode from '@/models/WorkflowTemplateNode';
import { Button } from '@/components/ui/button';
import TaskTemplate from '@/models/tasks/TaskTemplate';

interface Props {
  taskTemplates: TaskTemplate[];
}

export default function WorkflowTemplateBuilder(props: Props) {  

  // reserved node ids:
  const START_WORKFLOW_NODE_ID = "start_workflow";
  const END_WORKFLOW_NODE_ID = "end_workflow";  
  const MAINFLOW_TRIGGERING_TASK_NODE_ID = "mainflow_triggering_task_id";

  const ARROW_MARKER_END = {type: MarkerType.ArrowClosed, width: 10, height: 10, color: 'var(--foreground)' };

  const INITIAL_NODES: Node[] = [];
  const INITIAL_EDGES: Edge[] = [];

  const [nodes, setNodes, onNodesChange] = useNodesState(INITIAL_NODES);
  const [edges, setEdges, onEdgesChange] = useEdgesState(INITIAL_EDGES);

  const nodeTypes = useMemo(() => ({ taskNode: TaskNode, startNode: StartNode, endNode: EndNode, addTaskNode: AddTaskNode, inviteUserNode: InviteUserNode }), []);
  

  // preflow and mainflow tasks (i.e. preboarding and onboarding)  
  const [preflowTasks, setPreflowTasks] = useState<WorkflowTemplateNode[]>([]);
  const [mainflowTasks, setMainflowTasks] = useState<WorkflowTemplateNode[]>([]);



  function getNodeTypeFromTaskTemplateId(taskTemplateId: string) {
    switch(taskTemplateId) {
      case START_WORKFLOW_NODE_ID:
        return "startNode";
      case END_WORKFLOW_NODE_ID:
        return "endNode";
      case MAINFLOW_TRIGGERING_TASK_NODE_ID:
        return "inviteUserNode";
      default:
        return "taskNode"
    }
  }

  function createEdge(prevNode: Node, node: Node) {
    const edge: Edge = {
      id: `e${prevNode.id}-${node.id}`, source: prevNode.id, target: node.id, 
      markerEnd: ARROW_MARKER_END,
      style: { strokeWidth: 4, stroke: 'var(--foreground)'}
    };
    return edge;
  }

  function renderWorkflowNodes() {
    // loop through workflow nodes 
    let nodes: Node[] = [];
    let edges: Edge[] = [];
    // render workflow start node
    const startNode = {
      id: START_WORKFLOW_NODE_ID, type: "startNode", position: { x: 0, y: 0 },
      data: {}
    }
    nodes.push(startNode);  
    // render preflow tasks
    preflowTasks.forEach((task, index) => {
      // add task node
      const prevNode = nodes[nodes.length - 1]
      const nodeYPosition = prevNode.position.y + (index === 0 ? 100 : 175); 
      const taskNode: Node = {
        id: `preflow_${index}`, type: "taskNode", position: { x: 0, y: nodeYPosition},
        data: {
          taskTitle: task.taskTemplate.name,
          description: task.taskTemplate.description,
          deleteTask: () => { removeWorkflowNode(task.id) }
        } 
      };
      nodes.push(taskNode);
      // add edge between node and previous node
      edges.push(createEdge(prevNode, taskNode));
    });
    // add mainflow triggering task node
    var prevNode = nodes[nodes.length - 1]
    var nodeYPosition = prevNode.position.y + 175; 
    const triggeringTaskNode: Node = {
      id: MAINFLOW_TRIGGERING_TASK_NODE_ID,
      type: "inviteUserNode", position: { x: 0, y: nodeYPosition},
      data: {}
    }
    nodes.push(triggeringTaskNode);
    // add edge
    edges.push(createEdge(prevNode, triggeringTaskNode));
    // render mainflow tasks
    mainflowTasks.forEach((task, index) => {
      // add task node
      const prevNode = nodes[nodes.length - 1]
      const nodeYPosition = prevNode.position.y + (index === 0 ? 100 : 175); 
      const taskNode: Node = {
        id: `mainflow_${index}`, type: "taskNode", position: { x: 0, y: nodeYPosition},
        data: {
          taskTitle: task.taskTemplate.name,
          description: task.taskTemplate.description,
          deleteTask: () => { removeWorkflowNode(task.id) }
        } 
      };
      nodes.push(taskNode);
      // add edge between node and previous node
      edges.push(createEdge(prevNode, taskNode));
    })
    // add end of workflow node
    prevNode = nodes[nodes.length - 1]
    nodeYPosition = prevNode.position.y + 175; 
    const endWorkflowNode: Node = {
      id: END_WORKFLOW_NODE_ID,
      type: "endNode", position: { x: 0, y: nodeYPosition},
      data: {}
    }
    nodes.push(endWorkflowNode);
    // add edge
    edges.push(createEdge(prevNode, endWorkflowNode));

    setNodes(nodes);
    setEdges(edges);
  }

  function addWorkflowNode(taskId: string, type: "preflow" | "mainflow") {
    const taskTemplate = props.taskTemplates.find(i => i.id === taskId);
    if (taskTemplate == null) {
      return
    }
    const node: WorkflowTemplateNode = {
      id: crypto.randomUUID(),
      taskTemplate: taskTemplate
    };
    switch(type) {
      case "preflow":
        setPreflowTasks((prevState) => ([...prevState, node]));
        break;
      case "mainflow":
        setMainflowTasks((prevState) => ([...prevState, node]));
        break;
      default:
        return;
    }
  }

  function removeWorkflowNode(id: string) {
    // attempt to find node in preflow tasks
    setPreflowTasks((prevState) => (prevState.filter(i => i.id !== id)));
    setMainflowTasks((prevState) => (prevState.filter(i => i.id !== id)));
  }

  useEffect(() => {    
    renderWorkflowNodes();
  }, [preflowTasks, mainflowTasks]);

  return (
    <div className='flex flex-col gap-4'>
      <div style={{ width: '2000px', height: '1100px', margin: "auto"}}>
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
        </ReactFlow> 
      </div>
      <div className='flex flex-row gap-4 justify-center'>
        <Button onClick={() => {addWorkflowNode(props.taskTemplates[Math.floor(Math.random() * props.taskTemplates.length)].id, "preflow")}}>Add Preflow Task</Button>       
        <Button onClick={() => {addWorkflowNode(props.taskTemplates[Math.floor(Math.random() * props.taskTemplates.length)].id, "mainflow")}}>Add Mainflow Task</Button>       
      </div>
    </div>
  )
}
