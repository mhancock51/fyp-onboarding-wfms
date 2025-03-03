export default interface ListedTaskInstance {
    taskInstanceId: string;
    taskType: string;
    taskName: string;
    workflowName: string;
    status: string;
    dueIn: number;
    description: string;
}