export interface DecisionTaskInstanceTable {
  id: string;
  taskInstanceId: string;
  answer: number | null;
  taskTemplateId: string;
}