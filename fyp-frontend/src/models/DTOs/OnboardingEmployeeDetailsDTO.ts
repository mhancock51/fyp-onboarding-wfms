export default interface OnboardingEmployeeDetailsDTO {
  id: string;
  workflowInstanceId: string;
  displayName: string;
  emailAddress: string;
  departmentId: string;
  onboarderAccountId: string | null;
}