import { Badge } from "./ui/badge"
import { Button } from "./ui/button"
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu"
import NotificationDTO from "@/models/DTOs/NotificationDTO"
import { useEffect, useState } from "react"
import NoResults from "./NoResults"
import { Bell, X } from "lucide-react";
import moment from 'moment';
import { Spinner } from "./ui/spinner";
import Api from "@/api";
import { AxiosResponse } from "axios";
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
      const fetchedNotifications = (response.data.data as NotificationDTO[])
        .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
      
      dispatch(SET_NOTIFICATIONS([...fetchedNotifications]));        
    })
    .catch(() => {
      toast.error("Failed to load notifications");
    })
    .finally(() => {
      setLoading(false);
    })
  }

  async function deleteNotification(notification: NotificationDTO) {
    setLoading(true);
    await Api.notifications.deleteNotification(notification.id)
    .then(() => {
      void fetchNotifications();
    })
    .catch(() => {
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
    if (notifications.length === 0) {
      void fetchNotifications();
    }
    // start refresh timer
    const interval = setInterval(() => {
      void fetchNotifications();
    }, 15000);
    return () => clearInterval(interval);
  }, []);

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          variant="ghost"
          size="sm"
          className="cursor-pointer gap-2 outline-none"           
        >
          <Bell/><Badge className="py-[2px] w-[32px] bg-blue-500 rounded-full">{notifications.length}</Badge>              
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        className="mx-3 py-2 w-[--radix-dropdown-menu-trigger-width] w-[500px] rounded-lg max-h-[60vh] overflow-y-auto" side={"bottom"} align="end" sideOffset={4}
      >
        <DropdownMenuGroup className="gap-2 flex flex-col">
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
              <DropdownMenuItem key={notification.id} className="cursor-pointer border-accent rounded-none" onSelect={(e) => {e.preventDefault();}}>
                <div className="p-1 flex flex-col w-full relative flex flex-col gap-1 justify-between min-h-[50px]">
                  <div className="p-[4px] rounded-lg right absolute top-1 right-1 bg-background" onClick={() => {removeNotification(index);}}>
                    <X/>
                  </div>
                  <div className="flex flex-row w-full justify-start gap-2 items-start">
                    {
                      notification.status === "unseen" &&
                      <Badge className="px-4 rounded-full bg-blue-500 text-white">NEW</Badge>
                    }
                    <h1 className="text-base max-w-[435px] line-clamp-2">{notification.description}</h1>
                  </div>    
                  <div className="flex flex-row w-full justify-start">
                    <span className="text-gray-500 text-xs">{moment(notification.timestamp).fromNow()}</span>                        
                  </div>
                  <div className="flex flex-row gap-4 justify-start my-1 items-center">
                    {
                      notification.tags.map((tag, index) => (
                        <div key={index} className="bg-background border-blue-400 border-2 rounded-md px-4 py-[2px] min-w-[120px] flex flex-row justify-center">
                          <span key={index} className="text-xs text-blue-500">{tag}</span> 
                        </div>
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
  )
}