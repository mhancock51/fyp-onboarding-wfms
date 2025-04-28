import { Background, BackgroundVariant, Controls, Edge, MarkerType, Node, ReactFlow, useEdgesState, useNodesState } from '@xyflow/react'
import React, { useEffect, useMemo, useState } from 'react'
import AddTaskNode from './Nodes/AddTaskNode';
import EndNode from './Nodes/EndNode';
import InviteUserNode from './Nodes/InviteUserNode';
import StartNode from './Nodes/StartNode';
import TaskNode from './Nodes/TaskNode';
import TaskTemplate from '@/models/tasks/TaskTemplate';
import AddTaskToWorkflowDialog from '@/app/dialogs/AddTaskToWorkflowDialog';
import AccountDirectory from '@/models/AccountDirectory';
import WorkflowTemplateNode from '@/models/Workflows/WorkflowTemplateNode';
import clsx from 'clsx';
import { toast } from 'sonner';

interface Props {
  taskTemplates: TaskTemplate[];
  isOnboardingWorkflow: boolean;
  preflowNodes: WorkflowTemplateNode[];
  setPreflowNodes: React.Dispatch<React.SetStateAction<WorkflowTemplateNode[]>>;
  mainflowNodes: WorkflowTemplateNode[];
  setMainflowNodes: React.Dispatch<React.SetStateAction<WorkflowTemplateNode[]>>;
  isReadonly: boolean;
  className?: string;
}

export default function WorkflowTemplateBuilder(props: Props) {  

  // reserved node ids:
  const START_WORKFLOW_NODE_ID = "start_workflow";
  const END_WORKFLOW_NODE_ID = "end_workflow";  
  const MAINFLOW_TRIGGERING_TASK_NODE_ID = "mainflow_triggering_task_id";
  const ADD_TASK_NODE_ID = "add_task_node_id";

  const ARROW_MARKER_END = {type: MarkerType.ArrowClosed, width: 10, height: 10, color: 'var(--foreground)' };

  const INITIAL_NODES: Node[] = [];
  const INITIAL_EDGES: Edge[] = [];

  const [nodes, setNodes, onNodesChange] = useNodesState(INITIAL_NODES);
  const [edges, setEdges, onEdgesChange] = useEdgesState(INITIAL_EDGES);

  const nodeTypes = useMemo(() => ({ taskNode: TaskNode, startNode: StartNode, endNode: EndNode, addTaskNode: AddTaskNode, inviteUserNode: InviteUserNode }), []);

  const [openAddTaskDialog, setOpenAddTaskDialog] = useState<boolean>(false);
  const [selectedSection, setSelectedSection] = useState<"preflow" | "mainflow">("preflow");

  const [selectedNode, setSelectedNode] = useState<WorkflowTemplateNode | null>(null);


  function createEdge(prevNode: Node, node: Node) {
    const edge: Edge = {
      id: `e${prevNode.id}-${node.id}`, source: prevNode.id, target: node.id, 
      markerEnd: ARROW_MARKER_END,
      style: { strokeWidth: 4, stroke: 'var(--foreground)'}
    };
    return edge;
  }

  function canNodeMoveUp(node: WorkflowTemplateNode, nodes: WorkflowTemplateNode[], currentIndex: number): boolean {
    if (currentIndex === 0) return false;
    // if node before current node is one of the nodes dependency nodes then it cant
    if (node.taskDependencies.map((d => d.id)).includes(nodes[currentIndex - 1].id)) return false;

    return true;
  }

  function canNodeMoveDown(node: WorkflowTemplateNode, nodes: WorkflowTemplateNode[], currentIndex: number): boolean {
    if (currentIndex === nodes.length - 1) return false;
    // if node before current node is one of the nodes dependency nodes then it cant
    if (node.taskDependencies.map(d => d.id).includes(nodes[currentIndex + 1].id)) return false;
    // check that a node that is dependent on this node isn't just below it
    var dependentNodesIndexes = nodes.map((n, index) => (n.taskDependencies.includes(node) ? index : null)).filter(n => n !== null);
    if (dependentNodesIndexes.includes(currentIndex + 1)) return false; 

    return true;
  }

  function renderWorkflowNodes() {
    // loop through workflow nodes 
    let nodes: Node[] = [];
    let edges: Edge[] = [];

    // local scope functions
    const addNode = (node: Node) => {
      if (nodes.length > 0) {
        edges.push(createEdge(nodes[nodes.length - 1], node));        
      }
      nodes.push(node);
    }
    const createTaskNode = (nodeData: any, index: number, flowType: "preflow" | "mainflow") => ({
      id: `${flowType}_${index}`,
      type: "taskNode",
      position: { x: 0, y: nodes[nodes.length - 1].position.y + (index === 0 ? 100 : 175) },
      data: {
        node: nodeData,
        setSelectedNode: () => setSelectedNode(nodeData),
        isSelectedNode: nodeData.id === selectedNode?.id,
        selectedNode,
        deleteTask: () => removeWorkflowNode(nodeData.id),
        canMoveUp: canNodeMoveUp(nodeData, flowType === "preflow" ? props.preflowNodes : props.mainflowNodes, index),
        canMoveDown: canNodeMoveDown(nodeData, flowType === "preflow" ? props.preflowNodes : props.mainflowNodes, index),
        moveTaskUp: () => moveNodeUp(index, flowType),
        moveTaskDown: () => moveNodeDown(index, flowType),
        isReadOnly: props.isReadonly,
        isADependency: selectedNode?.taskDependencies.some(i => i.id === nodeData.id),
        removeDependency: () => removeDependencyOnNode(nodeData),
        addDependency: () => addDependencyOnNode(nodeData),
        clearSelectedNode: () => setSelectedNode(null),
        nodes: flowType === "preflow" ? props.preflowNodes : props.mainflowNodes
      }
    });

    const createAddTaskNode = (flowType: "preflow" | "mainflow") => ({
      id: `${flowType}_${ADD_TASK_NODE_ID}`,
      type: "addTaskNode",
      position: { x: 0, y: nodes[nodes.length - 1].position.y + 125 },
      data: {
        onClick: () => { setSelectedSection(flowType); setOpenAddTaskDialog(true); }
      }
    });

    // Add start node
    addNode({ id: START_WORKFLOW_NODE_ID, type: "startNode", position: { x: 0, y: 0 }, data: {} });

    // add preflow nodes
    if (props.isOnboardingWorkflow) {
      props.preflowNodes.forEach((node, index) => {
        addNode(createTaskNode(node, index, "preflow"));
      });
  
      if (!props.isReadonly) {
        addNode(createAddTaskNode("preflow"));
      }
  
      // Mainflow trigger node
      addNode({
        id: MAINFLOW_TRIGGERING_TASK_NODE_ID,
        type: "inviteUserNode",
        position: { x: 0, y: nodes[nodes.length - 1].position.y + 100 },
        data: {}
      });
    }

    // add mainflow nodes
    props.mainflowNodes.forEach((node, index) => {
      addNode(createTaskNode(node, index, "mainflow"));
    });

    if (!props.isReadonly) {
      addNode(createAddTaskNode("mainflow"));
    }
  
    // End node
    addNode({
      id: END_WORKFLOW_NODE_ID,
      type: "endNode",
      position: { x: 0, y: nodes[nodes.length - 1].position.y + 100 },
      data: {}
    });
  
    setNodes(nodes);
    setEdges(edges);    
  }

  function addWorkflowNode(taskTemplate: TaskTemplate, assignee: AccountDirectory, taskDependencies: WorkflowTemplateNode[], daysUntilDue: number | null, accountsToNotifyOnCompletion: AccountDirectory[]) {
    const node: WorkflowTemplateNode = {
      id: crypto.randomUUID(),
      taskTemplate: taskTemplate,
      assignee: assignee,
      taskDependencies: taskDependencies,
      daysUntilDue: daysUntilDue,
      accountsToNotify: accountsToNotifyOnCompletion.map((account) => account.id)    
    };
    switch(selectedSection) {
      case "preflow":
        props.setPreflowNodes((prevState) => ([...prevState, node]));
        break;
      case "mainflow":
        props.setMainflowNodes((prevState) => ([...prevState, node]));
        break;
      default:
        return;
    }
  }

  function moveItemBack<T,>(list: T[], index: number, direction: -1 | 1): T[] {
    const newIndex = index + direction;
    if (newIndex < 0 || newIndex >= list.length) return list; // Prevent out-of-bounds
    const newList = [...list];
    [newList[index], newList[newIndex]] = [newList[newIndex], newList[index]]; // Swap items
    return newList;
  };

  function moveNodeUp(index: number, section: "preflow" | "mainflow") {    
    if (section === "preflow" && index > 0) {
      props.setPreflowNodes((prevState) => moveItemBack(prevState, index, -1));      
    }
    else if(section === "mainflow" && index > 0) {
      props.setMainflowNodes((prevState) => moveItemBack(prevState, index, -1));
    }
  }

  function moveNodeDown(index: number, section: "preflow" | "mainflow") {    
    if (section === "preflow") {
      props.setPreflowNodes((prevState) => moveItemBack(prevState, index, 1));      
    }
    else if(section === "mainflow") {
      props.setMainflowNodes((prevState) => moveItemBack(prevState, index, 1));
    }
  }

  function removeWorkflowNode(workflowNodeId: string) {
    var updatedPreflowNodes = [...props.preflowNodes];
    var updateMainflowNodes = [...props.mainflowNodes];
    // find nodes that are dependent on that task
    var preflowDependentNodes = props.preflowNodes.filter(n => n.taskDependencies.map(d => d.id).includes(workflowNodeId));
    var mainflowDependentNodes = props.mainflowNodes.filter(n => n.taskDependencies.map(d => d.id).includes(workflowNodeId));
    if (preflowDependentNodes.length > 0 || mainflowDependentNodes.length > 0) {
      const confirmation = confirm(`${preflowDependentNodes.length + mainflowDependentNodes.length} nodes are dependent on this node are you sure you want to delete it?`);
      if (!confirmation) return;     
      // update these nodes to node be dependent on them      
      // remove task dependency in matching preflow nodes      
      updatedPreflowNodes = updatedPreflowNodes.map(node => {
        if (preflowDependentNodes.includes(node)) {
          // console.log(`Found dependent preflow node (removing dependency from list)`, node);          
          var updatedNode = {...node, taskDependencies: node.taskDependencies.filter(d => d.id !== workflowNodeId)}
          // console.log("Updated node:", updatedNode);
          return updatedNode;
        }
        else {
          return node;
        }
      })
      // remove task dependency in matching mainflow nodes      
      updateMainflowNodes = updateMainflowNodes.map(node => {
        if (mainflowDependentNodes.includes(node)) {
          // console.log(`Found dependent mainflow node (removing dependency from list)`, node);          
          var updatedNode = {...node, taskDependencies: node.taskDependencies.filter(d => d.id !== workflowNodeId)}
          // console.log("Updated node:", updatedNode);
          return updatedNode;
        }
        else {
          return node;
        }
      });

    }
    // remove node from lists
    updatedPreflowNodes = updatedPreflowNodes.filter(i => i.id !== workflowNodeId);
    updateMainflowNodes = updateMainflowNodes.filter(i => i.id !== workflowNodeId);
    // attempt to find node in preflow tasks
    props.setPreflowNodes(updatedPreflowNodes);
    props.setMainflowNodes(updateMainflowNodes);
  }

  function removeDependencyOnNode(workflowNode: WorkflowTemplateNode) {
    if (selectedNode === null) return;    
    // ensure selected node is dependent on this node
    if (!selectedNode.taskDependencies.some(d => d.id === workflowNode.id)) return;

    // update node in both lists (it will be in one or the other)
    props.setPreflowNodes((prevState) => prevState.map(node => node.id === selectedNode.id ? {...node, taskDependencies: node.taskDependencies.filter(d => d.id !== workflowNode.id)} : node))
    props.setMainflowNodes((prevState) => prevState.map(node => node.id === selectedNode.id ? {...node, taskDependencies: node.taskDependencies.filter(d => d.id !== workflowNode.id)} : node))    

    toast.success("Remove depedency between selected node and clicked node");
  }

  function addDependencyOnNode(workflowNode: WorkflowTemplateNode) {
    if (selectedNode === null) return;    
    // ensure selected node isn't dependent on this node
    if (selectedNode.taskDependencies.some(d => d.id === workflowNode.id)) return;

    // update node in both lists (it will be in one or the other)
    props.setPreflowNodes((prevState) => prevState.map(node => node.id === selectedNode.id ? {...node, taskDependencies: [...node.taskDependencies, workflowNode]} : node))
    props.setMainflowNodes((prevState) => prevState.map(node => node.id === selectedNode.id ? {...node, taskDependencies: [...node.taskDependencies, workflowNode]} : node))    
  
    toast.success("Created a depedency between selected node and clicked node");
  }

  useEffect(() => {
    // update selected node to reflect changes made to nodes (i.e. selected node might have been changed in some way when either node lists were updated)
    const updatedSelectedNode = props.mainflowNodes.concat(props.preflowNodes).filter(n => n.id === selectedNode?.id)[0];
    if (updatedSelectedNode) {
      setSelectedNode(updatedSelectedNode);
    }
  }, [props.mainflowNodes, props.preflowNodes]);

  useEffect(() => {        
    renderWorkflowNodes();
  }, [props.preflowNodes, props.mainflowNodes, props.isOnboardingWorkflow, props.isReadonly, selectedNode]);

  return (
    <div>
      <div className={clsx('min-w-full', props.className)}>
        <ReactFlow
          nodes={nodes}
          edges={edges}
          nodeTypes={nodeTypes}
          // onNodesChange={onNodesChange}
          // onEdgesChange={onEdgesChange}
          // onConnect={onConnect}
          defaultViewport={{x: 800, y: 200, zoom: 0.75}}
          nodeOrigin={[0.5, 0.5]}
        >
          <Background variant={BackgroundVariant.Dots} gap={12} size={1} />  
          <Controls />
        </ReactFlow> 
      </div>
      <AddTaskToWorkflowDialog open={openAddTaskDialog} setOpen={setOpenAddTaskDialog} onAddTask={addWorkflowNode} 
        isOnboardingWorkflow={props.isOnboardingWorkflow}
        existingTaskNodes={selectedSection === "preflow" ? props.preflowNodes : props.mainflowNodes}
        section={selectedSection}
      />
    </div>
  )
}
