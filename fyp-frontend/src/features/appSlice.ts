import AuthenticatedUser from "@/models/AuthenticatedUser";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface AppState {
    user: AuthenticatedUser | null;  
}

const initialState: AppState = {
    user: null
}

export const appSlice = createSlice({
    name: "app",
    initialState: initialState,
    reducers: {
        SET_USER: (state, action: PayloadAction<AuthenticatedUser | null>) => {
            state.user = action.payload;
        }
    }
});

export const {
    SET_USER
} = appSlice.actions;

export default appSlice.reducer;