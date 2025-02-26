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

  return (
    <Sidebar collapsible="icon" className="cursor-pointer">
      <SidebarHeader onClick={() => { navigate("/");}}>
        <SidebarGroupLabel style={{fontSize: "1.5em", textAlign: "center", margin: "auto"}}>{props.organisationName}</SidebarGroupLabel>
      </SidebarHeader>
      <SidebarSeparator/>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {items.map((item) => (
                <SidebarMenuItem key={item.title}>
                  <SidebarMenuButton asChild>
                    <a href={item.url}>
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
          name: "John Doe",
          email: "jdoe@enterprise.com",
          department: "IT Department"         
        }}/>
      </SidebarFooter>
    </Sidebar>
  )
}