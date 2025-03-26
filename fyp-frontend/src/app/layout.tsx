import { AppSidebar } from "@/components/app-sidebar";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { SET_OPEN_CREATE_DPT_DIALOG, SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG, SET_OPEN_INVITE_DIALOG } from "@/features/appSlice";
import { RootState } from "@/store";
import { useDispatch, useSelector } from "react-redux";
import { Outlet } from "react-router";
import InviteUserDialog from "./dialogs/InviteUserDialog";
import DepartmentCreationDialog from "./dialogs/DepartmentCreationDialog";
import CreateTaskTemplateDialog from "./dialogs/CreateTaskTemplateDialog";
import TaskTemplatesListDialog from "./dialogs/TaskTemplatesListDialog";
import CreateWorkflowInstanceDialog from "./dialogs/CreateWorkflowInstanceDialog";

export default function layout() {
  const dispatch = useDispatch();
  const openInviteDialog = useSelector((state: RootState) => state.app.openInviteDialog);  
  const openCreateDptDialog = useSelector((state: RootState) => state.app.openCreateDepartmentDialog);
  const openCreateTaskTemplateDialog = useSelector((state: RootState) => state.app.openCreateTaskTemplateDialog);
  
  const app = useSelector((state: RootState) => state.app);

  return (
    <SidebarProvider>
      <AppSidebar organisationName={"Ibcos"} />
      <main style={{padding: "8px", width: "100%"}}>
        <div className='m-4 flex flex-col gap-4'>
          <div className='rounded-3xl bg-accent p-8' >                    
            <Outlet />
          </div>
        </div>
      </main>
      <InviteUserDialog open={openInviteDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_INVITE_DIALOG(open));}}/>      
      <DepartmentCreationDialog open={openCreateDptDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_CREATE_DPT_DIALOG(open));}}/>
      <CreateTaskTemplateDialog open={openCreateTaskTemplateDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG(open))}}/>
      <TaskTemplatesListDialog open={app.openTaskTemplatesListDialog}/>
      <CreateWorkflowInstanceDialog/>
    </SidebarProvider>
  )
}
