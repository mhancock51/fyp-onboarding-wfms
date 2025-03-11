import { Background, BackgroundVariant, Controls, Edge, MarkerType, Node, ReactFlow, useEdgesState, useNodesState } from '@xyflow/react'
import React, { useEffect, useMemo, useState } from 'react'
import AddTaskNode from './Nodes/AddTaskNode';
import EndNode from './Nodes/EndNode';
import InviteUserNode from './Nodes/InviteUserNode';
import StartNode from './Nodes/StartNode';
import TaskNode from './Nodes/TaskNode';
import WorkflowTemplateNode from '@/models/WorkflowTemplateNode';
import { Button } from '@/components/ui/button';

export default function WorkflowTemplateBuilder() {
  // reserved task template Ids:
  const START_WORKFLOW_TASK_TEMPLATE_ID = "start_workflow";
  const END_WORKFLOW_TASK_TEMPLATE_ID = "end_workflow";
  const INVITE_ONBOARDER_TASK_TEMPLATED_ID = "invite_onboarder";

  const ARROW_MARKER_END = {type: MarkerType.ArrowClosed, width: 10, height: 10, color: 'var(--foreground)' };

  const INITIAL_NODES: Node[] = [];
  const INITIAL_EDGES: Edge[] = [];

  const [nodes, setNodes, onNodesChange] = useNodesState(INITIAL_NODES);
  const [edges, setEdges, onEdgesChange] = useEdgesState(INITIAL_EDGES);

  const nodeTypes = useMemo(() => ({ taskNode: TaskNode, startNode: StartNode, endNode: EndNode, addTaskNode: AddTaskNode, inviteUserNode: InviteUserNode }), []);

  const INITIAL_WF_NODES: WorkflowTemplateNode[] = [
    {
      taskTemplateId: START_WORKFLOW_TASK_TEMPLATE_ID,
      previousTaskTemplateId: ''      
    },
    {
      taskTemplateId: INVITE_ONBOARDER_TASK_TEMPLATED_ID,
      previousTaskTemplateId: START_WORKFLOW_TASK_TEMPLATE_ID      
    },
    {
      taskTemplateId: END_WORKFLOW_TASK_TEMPLATE_ID,
      previousTaskTemplateId: INVITE_ONBOARDER_TASK_TEMPLATED_ID      
    }
  ]

  const [workflowNodes, setWorkflowNodes] = useState<WorkflowTemplateNode[]>(INITIAL_WF_NODES);

  function getNodeTypeFromTaskTemplateId(taskTemplateId: string) {
    switch(taskTemplateId) {
      case START_WORKFLOW_TASK_TEMPLATE_ID:
        return "startNode";
      case END_WORKFLOW_TASK_TEMPLATE_ID:
        return "endNode";
      case INVITE_ONBOARDER_TASK_TEMPLATED_ID:
        return "inviteUserNode";
      default:
        return "taskNode"
    }
  }

  function renderWorkflowNodes() {
    // loop through workflow nodes 
    let nodes: Node[] = [];
    let edges: Edge[] = [];
    workflowNodes.forEach((wfNode, index) => {      
      var prevNode: WorkflowTemplateNode | undefined = undefined;
      prevNode = workflowNodes.find(i => i.taskTemplateId === wfNode.previousTaskTemplateId);    
      // get y pos of previous node
      const prevNodeYPos = nodes.find(i => i.id === wfNode.previousTaskTemplateId)?.position.y ?? 0;
      // create node for task    
      const node = {
        id: wfNode.taskTemplateId, type: getNodeTypeFromTaskTemplateId(wfNode.taskTemplateId),
        position: {
          x: 0,
          y: prevNodeYPos + 200
        },
        data: {}
      }
      nodes.push(node);
      // create link to previous node, if any    
      if (wfNode.previousTaskTemplateId !== "" && prevNode !== undefined) {    
        edges.push(
          {
            id: `edge-${prevNode.taskTemplateId}-to-${node.id}`,
            source: prevNode.taskTemplateId,
            target: node.id,
            markerEnd: ARROW_MARKER_END, style: { strokeWidth: 4, stroke: 'var(--foreground)'}
          }
        )
      }
      console.log("Nodes:", nodes);
      console.log("Edges:", edges);
      setNodes(nodes);
      setEdges(edges);
    })
  }

  function updateNodeInWFNodes(wfNodes: WorkflowTemplateNode[], updatedNode: WorkflowTemplateNode) {
    return wfNodes.map((wfNode) => (      
      wfNode.taskTemplateId === updatedNode.taskTemplateId ? updatedNode : wfNode
    ))
  }

  function addWorkflowNode(wfNode: WorkflowTemplateNode) {
    // add workflow node to list, link it to a valid previous task
    var updatedWfNodes = [...workflowNodes];
    console.log("test 3:", workflowNodes);
    updatedWfNodes.push(wfNode);
    // find the other task that had that prev task as its and make the new node that task's prev task
    var linkingNode = updatedWfNodes.find(i => i.previousTaskTemplateId === wfNode.previousTaskTemplateId);
    if (linkingNode) {
      linkingNode.previousTaskTemplateId = wfNode.taskTemplateId;
      console.log("test 1:",updatedWfNodes);
      updatedWfNodes = [...updateNodeInWFNodes(updatedWfNodes, linkingNode)];
      console.log("test 2:",updatedWfNodes);
    }    
    setWorkflowNodes(updatedWfNodes);
  }

  useEffect(() => {
    console.log("TESTTT:", workflowNodes);
    renderWorkflowNodes();
  }, [workflowNodes]);

  return (
    <div className='flex flex-col gap-4'>
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
        </ReactFlow> 
      </div>
      <Button onClick={() => {addWorkflowNode({
        taskTemplateId: Date.now().toString(),
        previousTaskTemplateId: workflowNodes[workflowNodes.length - 2].taskTemplateId
      })}}>Add Workflow Task</Button>       
    </div>
  )
}
