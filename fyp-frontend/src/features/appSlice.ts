import AuthenticatedUser from "@/models/AuthenticatedUser";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

const USER_DATA_STORAGE_KEY = "USER_DATA";

export interface AppState {
    user: AuthenticatedUser | null;  
}

const initialState: AppState = {
    user: loadUserFromLocalStorage()
}

export const appSlice = createSlice({
    name: "app",
    initialState: initialState,
    reducers: {
        SET_USER: (state, action: PayloadAction<AuthenticatedUser | null>) => {
            state.user = action.payload;            
            saveUserToLocalStorage(state.user);            
        }
    }
});

export const {
    SET_USER
} = appSlice.actions;

export default appSlice.reducer;

function saveUserToLocalStorage(user: AuthenticatedUser | null) {
    const jsonStr = JSON.stringify(user ?? "");
    localStorage.setItem(USER_DATA_STORAGE_KEY, jsonStr);
}

function loadUserFromLocalStorage(): AuthenticatedUser | null {
    const jsonStr = localStorage.getItem(USER_DATA_STORAGE_KEY);
    if (jsonStr === null) return null;

    return JSON.parse(jsonStr);
}