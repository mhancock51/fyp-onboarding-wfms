export default interface WorkflowTask {
    // ID of actual reusable task
    taskId: string;
    essential: boolean;
    assigneeUserId: string;
}