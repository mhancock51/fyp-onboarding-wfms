import { SidebarMenu, SidebarMenuButton, SidebarMenuItem, useSidebar } from './ui/sidebar'
import { ChevronsUpDown, LogOut, Settings } from 'lucide-react'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from './ui/dropdown-menu'
import { DropdownMenuGroup } from '@radix-ui/react-dropdown-menu'
import { useNavigate } from 'react-router'
import { useDispatch } from 'react-redux'
import { SET_USER } from '@/features/appSlice'

interface Props {
  user: {
    name: string
    email: string    
    department: string
  }
}

export default function SidebarUser(props: Props) {
  const { isMobile } = useSidebar()
  const navigate = useNavigate();

  const dispatch = useDispatch();

  function logOutUser() {
    dispatch(SET_USER(null));
  }

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <div className="grid flex-1 text-left text-sm leading-tight">
                <span className="truncate font-semibold">{props.user.name} ({props.user.department})</span>
                <span className="truncate text-xs">{props.user.email}</span>
              </div>
              <ChevronsUpDown className="ml-auto size-4" />
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg"
            side={isMobile ? "bottom" : "right"}
            align="end"
            sideOffset={4}
          >
            <DropdownMenuLabel className="p-0 font-normal">
              <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm" style={{cursor: "pointer"}}>
                <div className="grid flex-1 text-left text-sm leading-tight">
                  <span className="truncate font-semibold">{props.user.name}</span>
                  <span className="truncate text-xs">{props.user.email}</span>
                </div>
              </div>
            </DropdownMenuLabel>
            <DropdownMenuGroup>
              <DropdownMenuItem style={{cursor: "pointer"}} onClick={() => {navigate("/settings");}}>
                <Settings/> Settings
              </DropdownMenuItem>
            </DropdownMenuGroup>
            <DropdownMenuSeparator />
            <DropdownMenuItem style={{cursor: "pointer"}} onClick={logOutUser}>
              <LogOut />
              Log out
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}
