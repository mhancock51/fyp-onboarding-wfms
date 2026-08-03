import { SidebarMenu, SidebarMenuButton, SidebarMenuItem, useSidebar } from './ui/sidebar'
import { ChevronsUpDown, EllipsisVertical, LogOut, Settings, ShieldUser } from 'lucide-react'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from './ui/dropdown-menu'
import { DropdownMenuGroup } from '@radix-ui/react-dropdown-menu'
import { useNavigate } from 'react-router'
import { useDispatch } from 'react-redux'
import { SET_USER } from '@/features/appSlice'
import Utils from '@/util'
import ThemeToggle from './ThemeToggle'

interface Props {
  user: {
    name: string
    email: string    
    department: string
    isSupervisor: boolean;
  }
}

export default function SidebarUser(props: Props) {
  const { isMobile, state } = useSidebar()
  const isCollapsed = state === "collapsed" && !isMobile;
  const navigate = useNavigate();

  const dispatch = useDispatch();

  function logOutUser() {
    dispatch(SET_USER(null));
    Utils.clearLoginDetailsInLocalStorage();
    navigate("/login");
  }

  const dropdownContent = (
    <DropdownMenuContent
      className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg"
      side={isMobile ? "bottom" : "right"}
      align="end"
      sideOffset={4}
    >
      {isCollapsed && (
        <>
          <DropdownMenuLabel className="px-2 py-1.5">
            <div className="flex flex-col">
              <div className="flex flex-row items-center gap-1">
                {props.user.isSupervisor && <ShieldUser className="text-primary" size={16} />}
                <span className="truncate font-semibold text-sm">{props.user.name}</span>
              </div>
              <span className="truncate text-xs text-muted-foreground">{props.user.email}</span>
              <span className="truncate text-xs text-muted-foreground">{props.user.department}</span>
            </div>
          </DropdownMenuLabel>
          <DropdownMenuSeparator />
        </>
      )}
      <DropdownMenuGroup>
        <DropdownMenuItem style={{cursor: "pointer"}} onClick={() => {navigate("/settings");}}>
          <Settings/> Settings
        </DropdownMenuItem>
        <DropdownMenuItem className='cursor-pointer'>
          <ThemeToggle/>
        </DropdownMenuItem>
      </DropdownMenuGroup>
      <DropdownMenuSeparator />
      <DropdownMenuItem style={{cursor: "pointer"}} onClick={logOutUser}>
        <LogOut />
        Log out
      </DropdownMenuItem>
    </DropdownMenuContent>
  );

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            {isCollapsed ? (
              <SidebarMenuButton
                size="default"
                tooltip="Account"
                className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground outline-none mx-auto mb-2"
              >
                <EllipsisVertical className="size-4" />
              </SidebarMenuButton>
            ) : (
              <SidebarMenuButton
                size="lg"
                className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground outline-none"
              >
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <div className='flex flex-row items-center gap-1 justify-start'>
                    {
                      props.user.isSupervisor &&
                      <ShieldUser className='text-primary' size={20}/>
                    }
                    <span className="truncate font-semibold text-md">{props.user.name} ({props.user.department})</span>
                  </div>
                  <span className="truncate text-sm">{props.user.email}</span>
                </div>
                <ChevronsUpDown className="ml-auto size-4" />
              </SidebarMenuButton>
            )}
          </DropdownMenuTrigger>
          {dropdownContent}
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}