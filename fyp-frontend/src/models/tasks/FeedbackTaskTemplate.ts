export interface FeedbackTaskTemplate {
  id: string;
  taskTemplateId: string;
  questions: LikertQuestion[];
}

export interface LikertQuestion {
  question: string;
  likertScale: string[];
}