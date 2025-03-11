import React, { useCallback, useEffect, useMemo, useState } from 'react'
import { addEdge, Background, BackgroundVariant, Controls, MarkerType, Panel, ReactFlow, useEdgesState, useNodesState, Node, Edge } from '@xyflow/react';
import TaskNode from './Nodes/TaskNode';
import StartNode from './Nodes/StartNode';
import EndNode from './Nodes/EndNode';
import { Button } from '@/components/ui/button';
import WorkflowTemplate from '@/models/WorkflowTemplate';
import WorkflowTask from '@/models/WorkflowTask';
import AddTaskNode from './Nodes/AddTaskNode';
import AddTaskDialog from './AddTaskDialog';
import InviteUserNode from './Nodes/InviteUserNode';
import WorkflowTemplateBuilder from './WorkflowTemplateBuilder';

export default function CreateWorkflowPage() {

  return (
    <WorkflowTemplateBuilder/>
  )
}
