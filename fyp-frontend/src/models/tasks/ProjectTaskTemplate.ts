import ProjectObjective from "../ProjectObjective";

export default interface ProjectTaskTemplate {
  id: string;
  taskTemplateId: string;
  brief: string;
  objectives: ProjectObjective[];
  skills: string[];
}