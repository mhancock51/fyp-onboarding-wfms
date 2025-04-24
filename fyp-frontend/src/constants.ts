import AccountDirectory from "./models/AccountDirectory";

export const PLACEHOLDER_ONBOARDERS_ACCOUNT: AccountDirectory = {
  displayName: "Onboarder",
  id: 'onboarder_account_id',
  departmentId: '',
  departmentName: '',
  isSupervisor: false,
  emailAddress: ""
}

export const PLACEHOLDER_SUPERVISORS_ACCOUNT: AccountDirectory = {
  displayName: "Supervisor",
  id: "supervisors_account_id",
  departmentId: '',
  departmentName: '',
  isSupervisor: true,
  emailAddress: ""
}

export const TEMPLATE_ACCOUNTS = [
  PLACEHOLDER_ONBOARDERS_ACCOUNT,
  PLACEHOLDER_SUPERVISORS_ACCOUNT
]

export const TASK_TYPE_IDS = {
  PROJECT_TASK: "project-task",
  UPLOAD_DOCUMENT: "upload-document",
  READ_DOCUMENT: "read-document",
  CHECKLIST: "checklist",
  FEEDBACK_TASK: "feedback"
}