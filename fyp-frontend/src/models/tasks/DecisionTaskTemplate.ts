export interface DecisionTaskTemplateTable {
  id: string;
  taskTemplateId: string;
  question: string;
  answerA: string;
  answerB: string;
  subflowId: string;
}