import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from './ui/dropdown-menu'
import { Button } from './ui/button'
import { MoreHorizontal } from 'lucide-react'

interface Props {
  actions: DropdownAction[];
}

export default function TableActionsDropdown(props: Props) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" className="h-8 w-8 p-0">
          <span className="sr-only">Open menu</span>
          <MoreHorizontal />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className='w-[200px]'>
        <DropdownMenuLabel>Actions</DropdownMenuLabel>
        <DropdownMenuSeparator />
        {
          props.actions.map((action, index) => (
            <DropdownMenuItem key={index}
              onClick={action.onClick}
            >
              {action.label}
            </DropdownMenuItem>

          ))
        }                      
      </DropdownMenuContent>
    </DropdownMenu>
  )
}

export interface DropdownAction {
  label: string;
  onClick: () => void;
}
