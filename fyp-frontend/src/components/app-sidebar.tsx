import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarSeparator,
} from "@/components/ui/sidebar"
import { Building, ClipboardList, Home, ListTodo, Route, Settings, UserPlus, Users } from "lucide-react"
import SidebarUser from "./SidebarUser"
import { useNavigate } from "react-router"
import { useDispatch, useSelector } from "react-redux"
import { RootState } from "@/store"
import { SetStateAction, useState } from "react"
import InviteUserDialog from "@/app/dialogs/InviteUserDialog"
import { SET_OPEN_CREATE_DPT_DIALOG, SET_OPEN_CREATE_TASK_TEMPLATE_DIALOG, SET_OPEN_INVITE_DIALOG, SET_OPEN_TASK_TEMPLATES_LIST_DIALOG } from "@/features/appSlice"
import { title } from "process"
   
const mainItems = [
  {
    title: "My Tasks",
    url: "",
    icon: ClipboardList,
  },
  {
    title: "My Workflows",
    url: "/workflows",
    icon: Route,
  },
];

interface Props {
  organisationName: string;
}

export function AppSidebar(props: Props) {
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);

  const dispatch = useDispatch();

  const adminItems = [
    {
      title: "Task Templates",
      onClickAction: () => { dispatch(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(true));},
      icon: ListTodo
    },
    {
      title: "Create Workflow Template",
      onClickAction: () => { navigate("/create-workflow")},
      icon: Route
    },
    {
      title: "Invite User",
      onClickAction: () => { dispatch(SET_OPEN_INVITE_DIALOG(true)); },
      icon: UserPlus
    },
    {
      title: "Departments",
      onClickAction: () => { dispatch(SET_OPEN_CREATE_DPT_DIALOG(true)); },
      icon: Users
    },
    {
      title: "Organisation",
      onClickAction: () => {},
      icon: Building
    }
  ]

  return (
    <>
      <Sidebar collapsible="icon" className="cursor-pointer">
        <SidebarHeader onClick={() => { navigate("/");}}>
          <SidebarGroupLabel style={{fontSize: "1.5em", textAlign: "center", margin: "auto"}}>{props.organisationName}</SidebarGroupLabel>
          <SidebarSeparator/>
        </SidebarHeader>
        <SidebarContent>
          <SidebarGroup>
            <SidebarGroupLabel>Main</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {mainItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild>
                      <a onClick={() => {navigate(item.url)}}>
                        <item.icon />
                        <span>{item.title}</span>
                      </a>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
          {
            user?.isAdmin &&
            <SidebarGroup>
              <SidebarGroupLabel>Admin</SidebarGroupLabel>
              <SidebarGroupContent>
                <SidebarMenu>
                  {adminItems.map((item) => (
                    <SidebarMenuItem key={item.title}>
                      <SidebarMenuButton asChild>
                        <a onClick={item.onClickAction}>
                          <item.icon />
                          <span>{item.title}</span>
                        </a>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  ))}
                </SidebarMenu>
              </SidebarGroupContent>
            </SidebarGroup>
          }
        </SidebarContent>
        <SidebarFooter style={{padding: 0}}>
          <SidebarUser user={{
            name: user?.displayName ?? "",
            email: user?.emailAddress ?? "",
            department: user?.departmentName ?? ""         
          }}/>
        </SidebarFooter>
      </Sidebar>      
    </>
  )
}