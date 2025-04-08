import { SidebarMenu, SidebarMenuButton, SidebarMenuItem } from "./ui/sidebar";
import { Badge } from "./ui/badge"
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu"
import NotificationDTO from "@/models/DTOs/NotificationDTO"
import { useEffect, useState } from "react"
import NoResults from "./NoResults"
import { Bell, Dot, X } from "lucide-react";
import moment from 'moment';
import { Spinner } from "./ui/spinner";
import Api from "@/api";
import { Axios, AxiosResponse } from "axios";
import HTTPresponse from "@/models/HTTPresponse";
import { toast } from "sonner";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "@/store";
import { SET_NOTIFICATIONS } from "@/features/appSlice";

export default function NotificationsSidebarMenu() {
  const notifications = useSelector((state: RootState) => state.app.notifications);
  const dispatch = useDispatch();

  const [loading, setLoading] = useState<boolean>(false);

  async function fetchNotifications() {
    setLoading(true);
    await Api.notifications.fetchNotifications()
    .then((response: AxiosResponse<HTTPresponse<NotificationDTO[], string>>) => {
      dispatch(SET_NOTIFICATIONS(response.data.data as NotificationDTO[]));
    })
    .catch((error) => {
      toast.error("Failed to load notifications");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function deleteNotification(notification: NotificationDTO) {
    setLoading(true);
    await Api.notifications.deleteNotification(notification.id)
    .then((response: AxiosResponse<HTTPresponse<string, string>>) => {
      void fetchNotifications();
    })
    .catch((error) => {
      toast.error("Failed to delete notification");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  function removeNotification(index: number) {
    if (notifications.length <= index) return
    void deleteNotification(notifications[index]);
  }

  useEffect(() => {
    void fetchNotifications();
  }, []);

  return (
    <SidebarMenu>
      <SidebarMenuItem>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <SidebarMenuButton
              size="lg"
              className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
            >
              <Bell/> Notifications  <Badge className="p-1 min-w-[18px] bg-blue-500 rounded-full">{notifications.length}</Badge>
            </SidebarMenuButton>
          </DropdownMenuTrigger>
          <DropdownMenuContent
            className="mx-1 p-2 w-[--radix-dropdown-menu-trigger-width] w-[500px] rounded-lg max-h-[50vh] overflow-y-auto" side={"right"} align="end" sideOffset={4}
          >
            <DropdownMenuGroup>
              {
                loading &&
                <div className="flex flex-row gap-2 w-full items-center justify-center my-4">
                  <Spinner/> loading notifications...
                </div>
              }
              {
                !loading && notifications.length === 0 &&
                <NoResults text={"No notifications"}/>
              }
              {
                !loading && notifications.map((notification, index) => (
                  <DropdownMenuItem key={index} className="cursor-pointer flex flex-col gap-2 rounded-lg" onSelect={(e) => {e.preventDefault();}}>
                    <div className="p-1 flex flex-col w-full relative">
                      <div className="p-[6px] rounded-lg right absolute top-0 right-0 bg-background" onClick={() => {removeNotification(index);}}>
                        <X/>
                      </div>
                      <div className="flex flex-row w-full justify-between gap-4 items-start">
                        {
                          notification.status === "unseen" &&
                          <Badge className="p-1 rounded-full bg-blue-500 text-white">NEW</Badge>
                        }
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