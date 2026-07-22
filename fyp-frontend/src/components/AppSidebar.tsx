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
import { BadgeDollarSign, Building, ClipboardList, MessageSquareWarning, Route as WorkflowRouteIcon } from "lucide-react"
import SidebarUser from "./SidebarUser"
import { useNavigate } from "react-router"
import { useSelector } from "react-redux"
import { RootState } from "@/store"
import NotificationsSidebarMenu from "./NotificationSidebarMenu"
  

interface Props {
  organisationName: string;
  organisationLogoData: string | null;
  organisationLogoMimeType: string | null;
}

export function AppSidebar(props: Props) {
  const navigate = useNavigate();

  const user = useSelector((state: RootState) => state.app.user);

  const mainItems = [  
    {
      title: "Tasks",
      url: "",
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
      title: "Workflows",
      onClickAction: () => { navigate("/workflows/dashboard"); },
      icon: WorkflowRouteIcon
    },
    {
      title: "Organisation",
      onClickAction: () => { navigate("/organisation"); },
      icon: Building
    },
    {
      title: "Stripe Test",
      onClickAction: () => { navigate("/stripe-test"); },
      icon: BadgeDollarSign
    }
  ];

  const logoSrc = props.organisationLogoData && props.organisationLogoMimeType
    ? `data:${props.organisationLogoMimeType};base64,${props.organisationLogoData}`
    : null;

  return (
    <>
      <Sidebar collapsible="icon" className="cursor-pointer">
        <SidebarHeader onClick={() => { navigate("/");}}>
          <div className="w-full flex flex-col justify-center items-center gap-1 py-1">
            {
              logoSrc !== null
                ? <img src={logoSrc} alt={`${props.organisationName} logo`} className="max-h-32 w-auto object-contain" />
                : <h2 className="text-xl text-center">{props.organisationName}</h2>
            }
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
          {user?.isSupervisor && (
            <SidebarGroup>
              <SidebarGroupLabel>Management</SidebarGroupLabel>
              <SidebarGroupContent>
                <SidebarMenu>
                  {supervisorItems.map((item) => (
                    <SidebarMenuItem key={item.title}>
                      <SidebarMenuButton asChild className="min-h-[34px]">
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

