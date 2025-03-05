import AuthenticatedUser from "@/models/AuthenticatedUser";
import TaskType from "@/models/tasks/taskType";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

const USER_DATA_STORAGE_KEY = "USER_DATA";

export interface AppState {
    user: AuthenticatedUser | null;
    openInviteDialog: boolean;
    openCreateDepartmentDialog: boolean;
    taskTypes: TaskType[];
}

const initialState: AppState = {
    user: loadUserFromLocalStorage(),
    openInviteDialog: false,
    openCreateDepartmentDialog: false,
    taskTypes: []
}

export const appSlice = createSlice({
    name: "app",
    initialState: initialState,
    reducers: {
        SET_USER: (state, action: PayloadAction<AuthenticatedUser | null>) => {
            state.user = action.payload;            
            saveUserToLocalStorage(state.user);            
        },
        SET_OPEN_INVITE_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openInviteDialog = action.payload;
        },
        SET_OPEN_CREATE_DPT_DIALOG: (state, action: PayloadAction<boolean>) => {
            state.openCreateDepartmentDialog = action.payload;
        },
        SET_TASK_TYPES: (state, action: PayloadAction<TaskType[]>) => {
            state.taskTypes = action.payload;
        }
    }
});

export const {
    SET_USER, SET_OPEN_INVITE_DIALOG, SET_OPEN_CREATE_DPT_DIALOG, SET_TASK_TYPES
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