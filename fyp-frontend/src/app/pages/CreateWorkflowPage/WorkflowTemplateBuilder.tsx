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
import AddTaskToWorkflowDialog from '@/app/dialogs/AddTaskToWorkflowDialog';
import AccountDirectory from '@/models/AccountDirectory';

interface Props {
  taskTemplates: TaskTemplate[];
  isOnboardingWorkflow: boolean;
  preflowTasks: WorkflowTemplateNode[];
  setPreflowTasks: React.Dispatch<React.SetStateAction<WorkflowTemplateNode[]>>;
  mainflowTasks: WorkflowTemplateNode[];
  setMainflowTasks: React.Dispatch<React.SetStateAction<WorkflowTemplateNode[]>>;
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
    if (props.isOnboardingWorkflow) {
      // render preflow tasks (preboarding tasks)
      props.preflowTasks.forEach((task, index) => {
        console.log("TEST:", task.daysUntilDue);
        // add task node
        const prevNode = nodes[nodes.length - 1]
        const nodeYPosition = prevNode.position.y + (index === 0 ? 100 : 175); 
        const taskNode: Node = {
          id: `preflow_${index}`, type: "taskNode", position: { x: 0, y: nodeYPosition},
          data: {
            taskTitle: task.taskTemplate?.name,
            taskTypeId: task.taskTemplate?.taskTypeId,
            description: task.taskTemplate?.description,
            deleteTask: () => { removeWorkflowNode(task.id) },
            moveTaskUp: () => { moveNodeUp(index, "preflow");},
            moveTaskDown: () => { moveNodeDown(index, "preflow");},
            assignee: task.assignee?.displayName,
            taskDependencies: task.taskDependencies,
            daysUntilDue: task.daysUntilDue ?? null
          } 
        };
        nodes.push(taskNode);
        // add edge between node and previous node
        edges.push(createEdge(prevNode, taskNode));
      });
      // add button node to append a task
      prevNode = nodes[nodes.length - 1]    
      let addTaskNode: Node = {
        id: `preflow_${ADD_TASK_NODE_ID}`, type: "addTaskNode", position: {x: 0, y: prevNode.position.y + 125},
        data: {
          onClick: () => {setSelectedSection("preflow"); setOpenAddTaskDialog(true);}
        }
      }
      nodes.push(addTaskNode);
      edges.push(createEdge(prevNode, addTaskNode));
      // add mainflow triggering task node
      var prevNode = nodes[nodes.length - 1]
      var nodeYPosition = prevNode.position.y + 100; 
      const triggeringTaskNode: Node = {
        id: MAINFLOW_TRIGGERING_TASK_NODE_ID,
        type: "inviteUserNode", position: { x: 0, y: nodeYPosition},
        data: {}
      }
      nodes.push(triggeringTaskNode);
      // add edge
      edges.push(createEdge(prevNode, triggeringTaskNode));
    }

    // render mainflow tasks
    props.mainflowTasks.forEach((task, index) => {
      // add task node
      const prevNode = nodes[nodes.length - 1]
      const nodeYPosition = prevNode.position.y + (index === 0 ? 100 : 175); 
      const taskNode: Node = {
        id: `mainflow_${index}`, type: "taskNode", position: { x: 0, y: nodeYPosition},
        data: {
          taskTitle: task.taskTemplate?.name,
          taskTypeId: task.taskTemplate?.taskTypeId,
          description: task.taskTemplate?.description,
          deleteTask: () => { removeWorkflowNode(task.id) },
          moveTaskUp: () => { moveNodeUp(index, "mainflow");},
          moveTaskDown: () => { moveNodeDown(index, "mainflow");},
          assignee: task.assignee?.displayName,
          taskDependencies: task.taskDependencies
        } 
      };
      nodes.push(taskNode);
      // add edge between node and previous node
      edges.push(createEdge(prevNode, taskNode));
    })
    // add button node to append a task
    prevNode = nodes[nodes.length - 1]    
    var addTaskNode = {
      id: `mainflow_${ADD_TASK_NODE_ID}`, type: "addTaskNode", position: {x: 0, y: prevNode.position.y + 125},
      data: {
        onClick: () => {setSelectedSection("mainflow"); setOpenAddTaskDialog(true);}
      }
    }
    nodes.push(addTaskNode);
    edges.push(createEdge(prevNode, addTaskNode));
    // add end of workflow node
    prevNode = nodes[nodes.length - 1]
    nodeYPosition = prevNode.position.y + 100; 
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

  function addWorkflowNode(taskId: string, assignee: AccountDirectory, taskDependencies: WorkflowTemplateNode[], type: "preflow" | "mainflow", daysUntilDue: number | null) {
    const taskTemplate = props.taskTemplates.find(i => i.id === taskId);
    if (taskTemplate == null) {
      return
    }
    const node: WorkflowTemplateNode = {
      id: crypto.randomUUID(),
      taskTemplate: taskTemplate,
      assignee: assignee,
      taskDependencies: taskDependencies,
      daysUntilDue: daysUntilDue      
    };
    switch(type) {
      case "preflow":
        props.setPreflowTasks((prevState) => ([...prevState, node]));
        break;
      case "mainflow":
        props.setMainflowTasks((prevState) => ([...prevState, node]));
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
      props.setPreflowTasks((prevState) => moveItemBack(prevState, index, -1));      
    }
    else if(section === "mainflow" && index > 0) {
      props.setMainflowTasks((prevState) => moveItemBack(prevState, index, -1));
    }
  }

  function moveNodeDown(index: number, section: "preflow" | "mainflow") {    
    if (section === "preflow") {
      props.setPreflowTasks((prevState) => moveItemBack(prevState, index, 1));      
    }
    else if(section === "mainflow") {
      props.setMainflowTasks((prevState) => moveItemBack(prevState, index, 1));
    }
  }

  function removeWorkflowNode(id: string) {
    // attempt to find node in preflow tasks
    props.setPreflowTasks((prevState) => (prevState.filter(i => i.id !== id)));
    props.setMainflowTasks((prevState) => (prevState.filter(i => i.id !== id)));
  }

  function addTaskToWorkflow(taskTemplate: TaskTemplate, assingee: AccountDirectory, taskDependencies: WorkflowTemplateNode[], daysUntilDue: number | null) {
    taskDependencies = taskDependencies.filter(i => i !== null);
    addWorkflowNode(taskTemplate.id, assingee, taskDependencies, selectedSection, daysUntilDue);
  }

  useEffect(() => {    
    renderWorkflowNodes();
  }, [props.preflowTasks, props.mainflowTasks, props.isOnboardingWorkflow]);

  return (
    <div className='flex flex-col gap-4'>
      <div className='min-w-full' style={{height: '1000px', margin: "auto"}}>
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
      <AddTaskToWorkflowDialog open={openAddTaskDialog} setOpen={setOpenAddTaskDialog} onAddTask={addTaskToWorkflow} 
        isOnboardingWorkflow={props.isOnboardingWorkflow}
        existingTaskNodes={props.preflowTasks.concat(props.mainflowTasks)}
        section={selectedSection}
      />
    </div>
  )
}
