import { AppSidebar } from "@/components/app-sidebar";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { SET_OPEN_CREATE_DPT_DIALOG, SET_OPEN_INVITE_DIALOG } from "@/features/appSlice";
import { RootState } from "@/store";
import { useDispatch, useSelector } from "react-redux";
import { Outlet } from "react-router";
import InviteUserDialog from "./dialogs/InviteUserDialog";
import DepartmentCreationDialog from "./dialogs/DepartmentCreationDialog";

export default function layout() {
  const dispatch = useDispatch();
  const openInviteDialog = useSelector((state: RootState) => state.app.openInviteDialog);  
  const openCreateDptDialog = useSelector((state: RootState) => state.app.openCreateDepartmentDialog);

  return (
    <SidebarProvider>
      <AppSidebar organisationName={"Ibcos"} />
      <main style={{padding: "8px"}}>
        {/* <SidebarTrigger /> */}
        <Outlet />
      </main>
      <InviteUserDialog open={openInviteDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_INVITE_DIALOG(open));}}/>      
      <DepartmentCreationDialog open={openCreateDptDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_CREATE_DPT_DIALOG(open));}}/>

    </SidebarProvider>
  )
}
