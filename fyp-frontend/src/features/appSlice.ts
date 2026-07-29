import AccountDirectory from "@/models/AccountDirectory";
import AuthenticatedUser from "@/models/AuthenticatedUser";
import Department from "@/models/Department";
import IssueDTO from "@/models/DTOs/IssueDTO";
import NotificationDTO from "@/models/DTOs/NotificationDTO";
import WorkflowTemplateDTO from "@/models/DTOs/WorkflowTemplateDTO";
import Organisation from "@/models/Organisation";
import TaskTemplate from "@/models/tasks/TaskTemplate";
import TaskType from "@/models/tasks/TaskType";
import TenantSubscription from "@/models/TenantSubscription";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

const USER_DATA_STORAGE_KEY = "USER_DATA";

export interface AppState {
    user: AuthenticatedUser | null;
    organisation: Organisation | null;
    openAccountsDialog: boolean;
    openOrganisationDialog: boolean;
    openInviteDialog: boolean;
    openCreateDepartmentDialog: boolean;
    openCreateTaskTemplateDialog: boolean;
    openTaskTemplatesListDialog: boolean;
    openReportIssueDialog: boolean;
    openCreateWorkflowInstanceDialog: boolean;
    openUpdateTaskTemplateDialog: boolean;
    openViewWorkflowTemplateDialog: boolean;
    openManageSubscriptionDialog: boolean;
    openCancelSubscriptionDialog: boolean;
    departments: Department[];
    taskTypes: TaskType[];
    accountsDirectory: AccountDirectory[];
    taskTemplates: TaskTemplate[];
    selectedTaskTemplate: TaskTemplate | null;
    notifications: NotificationDTO[];
    workflowTemplates: WorkflowTemplateDTO[];
    tenantSubscription: TenantSubscription | null;
}

const initialState: AppState = {
    user: loadUserFromLocalStorage(),
    organisation: null,
    openAccountsDialog: false,
    openOrganisationDialog: false,
    openInviteDialog: false,
    openCreateDepartmentDialog: false,
    openCreateTaskTemplateDialog: false,
    openTaskTemplatesListDialog: false,
    openReportIssueDialog: false,
    openCreateWorkflowInstanceDialog: false,
    openUpdateTaskTemplateDialog: false,
    openViewWorkflowTemplateDialog: false,
    openManageSubscriptionDialog: false,
    openCancelSubscriptionDialog: false,
    taskTypes: [],
    accountsDirectory: [],
    taskTemplates: [],
    workflowTemplates: [],
    departments: [],
    selectedTaskTemplate: null,
    notifications: [],
    tenantSubscription: null
}

export const appSlice = createSlice({
    name: "app",
    initialState: initialState,
    reducers: {
        SET_USER: (state, action: PayloadAction<AuthenticatedUser | null>) => {
            const previousTenantId = state.user?.tenantId;
            const nextTenantId = action.payload?.tenantId;
            const isLogout = action.payload === null;
            const tenantChanged = previousTenantId !== undefined && nextTenantId !== undefined && previousTenantId !== nextTenantId;

            if (isLogout || tenantChanged) {
                state.organisation = null;
                state.tenantSubscription = null;
                state.departments = [];
                state.taskTypes = [];
                state.accountsDirectory = [];
                state.taskTemplates = [];
                state.selectedTaskTemplate = null;
                state.notifications = [];
                state.workflowTemplates = [];
            }

            state.user = action.payload;            
            saveUserToLocalStorage(state.user);            
        },
        SET_ORGANISATION: (state, action: PayloadAction<Organisation | null>) => {
            state.organisation = action.payload;
        },        
        SET_OPEN_ACCOUNTS_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openAccountsDialog = action.payload
        },
        SET_OPEN_ORGANISATION_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openOrganisationDialog = action.payload;
        },
        SET_OPEN_INVITE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openInviteDialog = action.payload;
        },
        SET_OPEN_CREATE_DPT_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openCreateDepartmentDialog = action.payload;
        },
        SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openCreateTaskTemplateDialog = action.payload;
        }, 
        SET_OPEN_TASK_TEMPLATES_LIST_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openTaskTemplatesListDialog = action.payload;
        },
        SET_OPEN_REPORT_ISSUE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openReportIssueDialog = action.payload
        },
        SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openCreateWorkflowInstanceDialog = action.payload;
        },
        SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openUpdateTaskTemplateDialog = action.payload;
        },
        SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openViewWorkflowTemplateDialog = action.payload;
        },
        SET_OPEN_MANAGE_SUBSCRIPTION_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openManageSubscriptionDialog = action.payload;
        },
        SET_OPEN_CANCEL_SUBSCRIPTION_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openCancelSubscriptionDialog = action.payload;
        },
        SET_DEPARTMENTS: (state, action: PayloadAction<Department[]>) => {
            state.departments = action.payload;
        },
        SET_TASK_TYPES: (state, action: PayloadAction<TaskType[]>) => {
            state.taskTypes = action.payload;
        },
        SET_ACCOUNTS_DIRECTORY: (state, action: PayloadAction<AccountDirectory[]>) => {
            state.accountsDirectory = action.payload;
        },
        SET_TASK_TEMPLATES: (state, action: PayloadAction<TaskTemplate[]>) => {
            state.taskTemplates = action.payload;
        },
        SET_SELECTED_TASK_TEMPLATE: (state, action: PayloadAction<TaskTemplate | null>) => {
            state.selectedTaskTemplate = action.payload;
        },
        SET_NOTIFICATIONS: (state, action: PayloadAction<NotificationDTO[]>) => {
            state.notifications = action.payload;
        },
        SET_WORKFLOW_TEMPLATES: (state, action: PayloadAction<WorkflowTemplateDTO[]>) => {
            state.workflowTemplates = action.payload;
        },
        SET_TENANT_SUBSCRIPTION: (state, action: PayloadAction<TenantSubscription | null>) => {
            state.tenantSubscription = action.payload;
        }
    }
});

export const {
    SET_USER, SET_ORGANISATION, SET_OPEN_ACCOUNTS_DIALOG, SET_OPEN_ORGANISATION_DIALOG, SET_OPEN_INVITE_DIALOG, SET_OPEN_CREATE_DPT_DIALOG, SET_TASK_TYPES, SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG,
    SET_OPEN_TASK_TEMPLATES_LIST_DIALOG, SET_ACCOUNTS_DIRECTORY, SET_TASK_TEMPLATES, SET_OPEN_REPORT_ISSUE_DIALOG, SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG, SET_DEPARTMENTS, SET_OPEN_UPDATE_TASK_TEMPLATE_DIALOG,
    SET_SELECTED_TASK_TEMPLATE, SET_NOTIFICATIONS, SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG, SET_WORKFLOW_TEMPLATES, SET_OPEN_MANAGE_SUBSCRIPTION_DIALOG, SET_OPEN_CANCEL_SUBSCRIPTION_DIALOG, SET_TENANT_SUBSCRIPTION
} = appSlice.actions;

export default appSlice.reducer;

function saveUserToLocalStorage(user: AuthenticatedUser | null) {
    if (user == null) {
        localStorage.removeItem(USER_DATA_STORAGE_KEY);
    }
    else {
        const jsonStr = JSON.stringify(user ?? "");
        localStorage.setItem(USER_DATA_STORAGE_KEY, jsonStr);
    }
}

function loadUserFromLocalStorage(): AuthenticatedUser | null {
    const jsonStr = localStorage.getItem(USER_DATA_STORAGE_KEY);
    if (jsonStr === null || jsonStr === "")  return null;

    return JSON.parse(jsonStr);
}