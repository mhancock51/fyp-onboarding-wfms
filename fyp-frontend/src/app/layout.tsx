import { AppSidebar } from "@/components/AppSidebar";
import { SidebarProvider } from "@/components/ui/sidebar";
import { SET_OPEN_CREATE_DPT_DIALOG, SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG, SET_OPEN_INVITE_DIALOG } from "@/features/appSlice";
import { RootState } from "@/store";
import { useDispatch, useSelector } from "react-redux";
import { Outlet } from "react-router";
import InviteUserDialog from "./dialogs/InviteUserDialog";
import DepartmentCreationDialog from "./dialogs/DepartmentCreationDialog";
import CreateTaskTemplateDialog from "./dialogs/taskTemplateDialogs/CreateTaskTemplateDialog";
import TaskTemplatesListDialog from "./dialogs/TaskTemplatesListDialog";
import CreateWorkflowInstanceDialog from "./dialogs/CreateWorkflowInstanceDialog";
import ManageAccountsDialog from "./dialogs/ManageAccountsDialog";
import OrganisationDialog from "./dialogs/OrganisationDialog";
import UpdateTaskTemplateDialog from "./dialogs/taskTemplateDialogs/UpdateTaskTemplateDialog";
import ViewWorkflowTemplateDialog from "./dialogs/ViewWorkflowTemplateDialog";
import ManageSubscriptionDialog from "./dialogs/ManageSubscriptionDialog";
import CancelSubscriptionDialog from "./dialogs/CancelSubscriptionDialog";
import Navbar from "@/components/Navbar";

export default function layout() {
  const dispatch = useDispatch();
  
  const app = useSelector((state: RootState) => state.app);

  return (
    <SidebarProvider>
      <AppSidebar
        organisationName={app.organisation?.name ?? "Organisation"}
        organisationLogoData={app.organisation?.logoImageData ?? null}
        organisationLogoMimeType={app.organisation?.logoImageMimeType ?? null}
      />
      <div className="flex flex-col w-full">
        <Navbar/>
        <main className='p-4 m-b-0 flex flex-col h-[93vh] overflow-y-auto'>
          <Outlet />                  
        </main>
      </div>
      <InviteUserDialog open={app.openInviteDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_INVITE_DIALOG(open));}}/>      
      <DepartmentCreationDialog open={app.openCreateDepartmentDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_CREATE_DPT_DIALOG(open));}}/>
      <CreateTaskTemplateDialog open={app.openCreateTaskTemplateDialog} setOpenDialog={(open: boolean) => {dispatch(SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG(open))}}/>
      <TaskTemplatesListDialog open={app.openTaskTemplatesListDialog}/>            
      <CreateWorkflowInstanceDialog/>
      <OrganisationDialog/>
      <ManageAccountsDialog/>
      <UpdateTaskTemplateDialog/> 
      <ViewWorkflowTemplateDialog/>
      <ManageSubscriptionDialog/>
      <CancelSubscriptionDialog/>
    </SidebarProvider>
  )
}
