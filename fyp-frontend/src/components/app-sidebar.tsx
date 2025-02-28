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
import { ClipboardList, Home, Route, Settings } from "lucide-react"
import SidebarUser from "./SidebarUser"
import { useNavigate } from "react-router"
import { useSelector } from "react-redux"
import { RootState } from "@/store"
   
const items = [
  {
    title: "My Tasks",
    url: "/tasks",
    icon: ClipboardList,
  },
  {
    title: "My Workflows",
    url: "/workflows",
    icon: Route,
  },
]

interface Props {
  organisationName: string;
}

export function AppSidebar(props: Props) {
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);

  console.log(user);  

  return (
    <Sidebar collapsible="icon" className="cursor-pointer">
      <SidebarHeader onClick={() => { navigate("/");}}>
        <SidebarGroupLabel style={{fontSize: "1.5em", textAlign: "center", margin: "auto"}}>{props.organisationName}</SidebarGroupLabel>
        <SidebarSeparator/>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {items.map((item) => (
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
      </SidebarContent>
      <SidebarFooter style={{padding: 0}}>
        <SidebarUser user={{
          name: user?.displayName ?? "",
          email: user?.emailAddress ?? "",
          department: user?.departmentName ?? ""         
        }}/>
      </SidebarFooter>
    </Sidebar>
  )
}