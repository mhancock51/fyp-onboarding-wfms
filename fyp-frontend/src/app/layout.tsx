import { AppSidebar } from "@/components/app-sidebar";
import { SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar";
import { Outlet } from "react-router";

export default function layout() {
  return (
    <SidebarProvider>
      <AppSidebar organisationName={"Ibcos"} />
      <main style={{padding: "8px"}}>
        {/* <SidebarTrigger /> */}
        <Outlet />
      </main>
    </SidebarProvider>
  )
}
