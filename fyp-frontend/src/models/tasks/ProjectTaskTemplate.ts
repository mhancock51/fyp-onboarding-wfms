import ProjectObjective from "./ProjectObjective";
import ProjectSupportLink from "./ProjectSupportLink";

export default interface ProjectTaskTemplate {
  id: string;
  taskTemplateId: string;
  brief: string;
  deliverable: string;
  objectives: ProjectObjective[];
  supportLinks: ProjectSupportLink[];
  skills: string[];
}