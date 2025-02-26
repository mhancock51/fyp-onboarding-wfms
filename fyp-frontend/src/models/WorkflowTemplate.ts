import WorkflowTask from "./WorkflowTask";

export default interface WorkflowInterface {
    name: string;
    description: string;
    tasks: WorkflowTask[];
}