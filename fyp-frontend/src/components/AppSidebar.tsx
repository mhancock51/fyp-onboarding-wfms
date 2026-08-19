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
import { Building, ClipboardList, MessageSquareWarning, Route as WorkflowRouteIcon } from "lucide-react"
import SidebarUser from "./SidebarUser"
import { useLocation, useNavigate } from "react-router"
import { useSelector } from "react-redux"
import { RootState } from "@/store"
  

interface Props {
  organisationName: string;
  organisationLogoData: string | null;
  organisationLogoMimeType: string | null;
}

export function AppSidebar({organisationName, organisationLogoData, organisationLogoMimeType}: Props) {
  const navigate = useNavigate();
  const { pathname } = useLocation();

  const user = useSelector((state: RootState) => state.app.user);

  const mainItems = [  
    {
      title: "Tasks",
      url: "/",
      icon: ClipboardList,
    },
    {
      title: "Workflows",
      url: "/workflows",
      icon: WorkflowRouteIcon,
    },
    {
      title: "Task Issues",
      url: "/issues",
      icon: MessageSquareWarning
    }
  ];

  const supervisorItems = [
    {
      title: "Workflows Dashboard",
      onClickAction: () => { navigate("/workflows-dashboard"); },
      matchUrl: "/workflows-dashboard",
      icon: WorkflowRouteIcon
    },
    {
      title: "Organisation",
      onClickAction: () => { navigate("/organisation"); },
      matchUrl: "/organisation",
      icon: Building
    },
  ];

  const isActive = (url: string) => {
    if (url === "/") return pathname === "/";
    return pathname === url || pathname.startsWith(`${url}/`);
  };

  const logoSrc = organisationLogoData && organisationLogoMimeType
    ? `data:${organisationLogoMimeType};base64,${organisationLogoData}`
    : null;

  return (
    <>
      <Sidebar collapsible="icon" className="cursor-pointer">
        <SidebarHeader onClick={() => { navigate("/");}}>
          <div className="w-full flex flex-col justify-center items-center gap-1 py-1">
            {
              logoSrc !== null
                ? <img src={logoSrc} alt={`${organisationName} logo`} className="max-h-32 w-auto object-contain" />
                : <h2 className="text-xl text-center">{organisationName}</h2>
            }
            <SidebarSeparator/>
          </div>
        </SidebarHeader>
        <SidebarContent className="gap-0">
          <SidebarGroup>
            <SidebarGroupContent>
              <SidebarMenu>
                {mainItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton asChild isActive={isActive(item.url)} className="min-h-[34px]">                    
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
          {user?.isSupervisor && (
            <SidebarGroup>
              <SidebarGroupLabel>Management</SidebarGroupLabel>
              <SidebarGroupContent>
                <SidebarMenu>
                  {supervisorItems.map((item) => (
                    <SidebarMenuItem key={item.title}>
                      <SidebarMenuButton asChild isActive={isActive(item.matchUrl)} className="min-h-[34px]">
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
          )}
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

