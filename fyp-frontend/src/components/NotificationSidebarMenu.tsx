import { Badge } from "./ui/badge"
import { Button } from "./ui/button"
import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuSeparator, DropdownMenuTrigger } from "./ui/dropdown-menu"
import NotificationDTO from "@/models/DTOs/NotificationDTO"
import { useCallback, useEffect, useRef, useState } from "react"
import NoResults from "./NoResults"
import { Bell, CheckCheck, X } from "lucide-react";
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

  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const previousUnseenCount = useRef(0);

  const unseenCount = notifications.filter(n => n.status === "unseen").length;
  const hasNew = unseenCount > previousUnseenCount.current;

  // Track when new notifications arrive for pulse animation
  useEffect(() => {
    previousUnseenCount.current = unseenCount;
  }, [unseenCount]);

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
    });
  }

  async function markAsRead(notification: NotificationDTO) {
    if (notification.status !== "unseen") return;
    // Optimistic update
    const updated = notifications.map(n =>
      n.id === notification.id ? { ...n, status: "seen" as const } : n
    );
    dispatch(SET_NOTIFICATIONS(updated));
    // Fire-and-forget API call
    Api.notifications.markAsRead(notification.id).catch(() => {});
  }

  async function markAllAsRead() {
    const updated = notifications.map(n =>
      n.status === "unseen" ? { ...n, status: "seen" as const } : n
    );
    dispatch(SET_NOTIFICATIONS(updated));
    Api.notifications.markAllAsRead().catch(() => {});
  }

  async function dismissNotification(notification: NotificationDTO) {
    // Optimistic removal
    const updated = notifications.filter(n => n.id !== notification.id);
    dispatch(SET_NOTIFICATIONS(updated));
    // API call
    Api.notifications.deleteNotification(notification.id)
    .catch(() => {
      toast.error("Failed to dismiss notification");
      void fetchNotifications(); // refetch on failure to restore state
    });
  }

  const handleOpenChange = useCallback((isOpen: boolean) => {
    setOpen(isOpen);
  }, []);

  useEffect(() => {    
    if (notifications.length === 0) {
      void fetchNotifications();
    }
    const interval = setInterval(() => {
      void fetchNotifications();
    }, 15000);
    return () => clearInterval(interval);
  }, []);

  return (
    <DropdownMenu open={open} onOpenChange={handleOpenChange}>
      <DropdownMenuTrigger asChild>
        <Button
          variant="ghost"
          size="sm"
          className="cursor-pointer gap-2 outline-none relative"           
        >
          <div className="relative">
            <Bell className={hasNew ? "animate-pulse text-primary" : ""} />
          </div>
          <Badge className="py-[2px] min-w-[24px] bg-primary rounded-full">{unseenCount}</Badge>              
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        className="mx-3 py-2 w-[500px] rounded-lg max-h-[60vh] overflow-y-auto bg-secondary" side="bottom" align="end" sideOffset={4}
      >
        {unseenCount > 0 && (
          <>
            <DropdownMenuItem
              className="cursor-pointer justify-center text-sm text-primary font-medium"
              onSelect={(e) => { e.preventDefault(); markAllAsRead(); }}
            >
              <CheckCheck className="size-4" />
              Mark all as read
            </DropdownMenuItem>
            <DropdownMenuSeparator />
          </>
        )}
        <DropdownMenuGroup className="gap-2 flex flex-col">
          {loading && (
            <div className="flex flex-row gap-2 w-full items-center justify-center my-4">
              <Spinner/> loading notifications...
            </div>
          )}
          {!loading && notifications.length === 0 && (
            <NoResults text="No notifications"/>
          )}
          {!loading && notifications.map((notification) => (
            <DropdownMenuItem
              key={notification.id}
              className="cursor-pointer rounded-none"
              onSelect={(e) => {
                e.preventDefault();
                markAsRead(notification);
              }}
            >
              <div className={[
                "p-2 flex flex-col w-full relative gap-1 justify-between min-h-[50px] rounded-md transition-colors",
                notification.status === "unseen" ? "bg-primary/10" : ""
              ].join(" ")}>
                <div
                  className="p-[4px] rounded-lg absolute top-1 right-1 bg-background hover:bg-accent z-10"
                  onClick={(e) => { e.stopPropagation(); dismissNotification(notification); }}
                >
                  <X className="size-3" />
                </div>
                <div className="flex flex-row w-full justify-start gap-2 items-start pr-6">
                  {notification.status === "unseen" && (
                    <Badge className="px-2 rounded-full bg-primary text-white text-xs shrink-0">NEW</Badge>
                  )}
                  <span className="text-sm line-clamp-2">{notification.description}</span>
                </div>    
                {notification.tags.length > 0 && (
                  <div className="flex flex-row gap-2 justify-start items-center flex-wrap">
                    {notification.tags.map((tag, i) => (
                      <div key={i} className="bg-background border-primary/60 border rounded-md px-3 py-[1px]">
                        <span className="text-xs text-primary">{tag}</span> 
                      </div>
                    ))}
                <div className="flex flex-row w-full justify-start">
                  <span className="text-gray-500 text-xs">{moment(notification.timestamp).fromNow()}</span>                        
                </div>
                  </div>
                )}
              </div>
            </DropdownMenuItem>
          ))}
        </DropdownMenuGroup>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}