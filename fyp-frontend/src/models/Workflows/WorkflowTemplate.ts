import WorkflowTemplateNode from './WorkflowTemplateNode';

export default interface WorkflowTemplate {
  name: string;
  description: string;
  isOnboardingWf: boolean;
  preflowTaskNodes: WorkflowTemplateNode[];
  mainflowTaskNodes: WorkflowTemplateNode[];      
}