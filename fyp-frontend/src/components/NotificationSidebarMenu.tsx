import { SidebarMenu, SidebarMenuButton, SidebarMenuItem, useSidebar } from "./ui/sidebar";
import { Badge } from "./ui/badge"
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuLabel, DropdownMenuTrigger } from "./ui/dropdown-menu"
import NotificationDTO from "@/models/DTOs/NotificationDTO"
import { useState } from "react"
import NoResults from "./NoResults"
import { Bell, Dot, X } from "lucide-react";
import moment from 'moment';

export default function NotificationsSidebarMenu() {
  const { isMobile } = useSidebar()

  const INITIAL_NOTIFICATIONS: NotificationDTO[] = [
    {
      description: "Onboarding workflow instance to onboard John Doe started!",
      tags: ["Workflows", "John Doe"],
      timestamp: new Date(1744115323 * 1000),
      state: "new",
    },
    {
      description: "Jane White just completed an onboarding task",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "new",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "new",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "new",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    },
    {
      description: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec mauris sapien, dapibus vitae mattis ac, maximus sit amet felis. Fusce vestibulum quis leo nec laoreet. Maecenas placerat molestie aliquam. Maecenas pulvinar elementum felis, vel aliquet turpis dapibus a. Phasellus sit amet scelerisque turpis. Donec sed urna massa. Suspendisse diam est, aliquet ac tortor fringilla, ultricies consequat orci. In egestas tortor mauris. Quisque lorem nibh, gravida ut leo nec, mollis varius nisl.",
      tags: ["Workflows", "Upload Employee's Passport"],
      timestamp: new Date(1744115323 * 1000),
      state: "",
    }    
  ];
  const [notifications, setNotifications] = useState<NotificationDTO[]>(INITIAL_NOTIFICATIONS);

  function removeNotification(index: number) {
    var updatedNotifications = [...notifications];
    updatedNotifications.splice(index, 1);    
    setNotifications(updatedNotifications);
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
              <Bell/> Notifications  <Badge className="p-1 min-w-[18px] bg-blue-500 rounded-full">{3}</Badge>
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="mx-1 p-2 w-[--radix-dropdown-menu-trigger-width] w-[500px] rounded-lg max-h-[50vh] overflow-y-auto" side={"right"} align="end" sideOffset={4}
          >
            <DropdownMenuGroup>
              {
                notifications.length === 0 &&
                <NoResults text={"No notifications"}/>
              }
              {
                notifications.map((notification, index) => (
                  <DropdownMenuItem key={index} className="cursor-pointer flex flex-col gap-2 rounded-lg" onSelect={(e) => {e.preventDefault();}}>
                    <div className="p-1 flex flex-col w-full relative">
                      <div className="p-[6px] rounded-lg right absolute top-0 right-0 bg-background" onClick={() => {removeNotification(index);}}>
                        <X/>
                      </div>
                      <div className="flex flex-row w-full justify-between gap-4 items-start">
                        <h1 className="text-base font-semibold max-w-[425px] line-clamp-2">{notification.description}</h1>
                      </div>    
                      <div className="flex flex-row gap-2 w-full justify-center items-center text-sm my-1">
                        {
                          moment(notification.timestamp).fromNow()
                        }
                        {
                          notification.tags.length > 0 && <Dot/>
                        }
                        {
                          notification.tags.map((tag, index) => (
                            <>
                              <span key={index} className="text-gray-700">{tag}</span> 
                              {
                                index !== notification.tags.length - 1 &&
                                <div key={index}>
                                  <Dot/>
                                </div>
                              } 
                            </>
                          ))
                        }
                      </div>
                    </div>
                  </DropdownMenuItem>    
                )
                )
              }
            </DropdownMenuGroup>
            
          </DropdownMenuContent>
        </DropdownMenu>
      </SidebarMenuItem>
    </SidebarMenu>
  )
}