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
  useSidebar,
} from "@/components/ui/sidebar"
import { Bell, Blocks, Building, ChartNoAxesColumn, ClipboardList, Dot, Home, ListTodo, MessageSquareWarning, Route, Settings, Trash2, UserPlus, Users, X } from "lucide-react"
import SidebarUser from "./SidebarUser"
import { useNavigate } from "react-router"
import { useDispatch, useSelector } from "react-redux"
import { RootState } from "@/store"
import { SET_OPEN_ACCOUNTS_DIALOG, SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG, SET_OPEN_INVITE_DIALOG, SET_OPEN_ORGANISATION_DIALOG, SET_OPEN_TASK_TEMPLATES_LIST_DIALOG, SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG } from "@/features/appSlice"
import NotificationsSidebarMenu from "./NotificationSidebarMenu"
  

interface Props {
  organisationName: string;
}

export function AppSidebar(props: Props) {
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);

  const dispatch = useDispatch();

  const mainItems = [  
    {
      title: "Tasks",
      url: "",
      icon: ClipboardList,
    },
    {
      title: "Workflows",
      url: "/workflows",
      icon: Route,
    },
    {
      title: "Task Issues",
      url: "/issues",
      icon: MessageSquareWarning
    }
  ];

  const adminItems = [    
    {
      title: "Invite User",
      onClickAction: () => { dispatch(SET_OPEN_INVITE_DIALOG(true)); },
      icon: UserPlus
    },
    {
      title: "Manage Accounts",
      onClickAction: () => { dispatch(SET_OPEN_ACCOUNTS_DIALOG(true)); },
      icon: Users
    },
    {
      title: "Manage Organisation",
      onClickAction: () => { dispatch(SET_OPEN_ORGANISATION_DIALOG(true));},
      icon: Building
    }
  ];

  const supervisorItems = [
    {
      title: "Workflows Dashboard",
      onClickAction: () => { navigate("/workflows/dashboard")},
      icon: ChartNoAxesColumn
    }, 
    {
      title: "Start A Workflow Instance",
      onClickAction: () => { dispatch(SET_OPEN_CREATE_WORKFLOW_INSTANCE_DIALOG(true));},
      icon: Route
    },
    {
      title: "Task Templates",
      onClickAction: () => { dispatch(SET_OPEN_TASK_TEMPLATES_LIST_DIALOG(true));},
      icon: ListTodo
    },
    {
      title: "Workflow Templates",
      onClickAction: () => { dispatch(SET_OPEN_VIEW_WORKFLOW_TEMPLATE_DIALOG(true));},
      icon: Route
    },  
    {
      title: "Build a Workflow",
      onClickAction: () => { navigate("/workflows/build")},
      icon: Blocks
    },
  ]

  return (
    <>
      <Sidebar collapsible="icon" className="cursor-pointer">
        <SidebarHeader onClick={() => { navigate("/");}}>
          <div className="w-full flex flex-col justify-center items-center gap-1 py-1">
            <h2 className="text-xl text-center">{props.organisationName}</h2>
            <SidebarSeparator/>
          </div>
        </SidebarHeader>
        <SidebarContent className="gap-0">
          <NotificationsSidebarMenu/>
          <SidebarGroup>
            <SidebarGroupLabel>Main</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {mainItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild className="min-h-[34px]">                    
                      <a onClick={() => {navigate(item.url)}} >
                        <item.icon/>
                        {item.title}
                      </a>                      
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}                
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
          {
            /* change this to isSupervisor check */
            user?.isSupervisor &&
            <SidebarGroup>
              <SidebarGroupLabel>Supervisor</SidebarGroupLabel>
              <SidebarGroupContent>
                <SidebarMenu>
                  {
                    supervisorItems.map((item, index) => (
                      <SidebarMenuItem key={index}>
                        <SidebarMenuButton asChild className="min-h-[34px]"> 
                        <a onClick={item.onClickAction}>
                          <item.icon />
                          <span>{item.title}</span>
                        </a>
                      </SidebarMenuButton>
                      </SidebarMenuItem>
                    ))  
                  }
                </SidebarMenu>
              </SidebarGroupContent>
            </SidebarGroup>
          }
          {
            user?.isSupervisor &&
            <SidebarGroup>
              <SidebarGroupLabel>Admin</SidebarGroupLabel>
              <SidebarGroupContent>
                <SidebarMenu>
                  {adminItems.map((item) => (
                    <SidebarMenuItem key={item.title}>
                      <SidebarMenuButton asChild className="min-h-[38px]"> 
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
        <SidebarFooter className="p-0 outline-none">          
          <SidebarUser user={{
            name: user?.displayName ?? "",
            email: user?.emailAddress ?? "",
            department: user?.departmentName ?? "",   
            isSupervisor: user?.isSupervisor ?? false,      
          }}/>
        </SidebarFooter>
      </Sidebar>      
    </>
  )
}

