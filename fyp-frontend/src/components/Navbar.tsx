import { SidebarTrigger } from "./ui/sidebar";
import { usePageTitle } from "@/hooks/usePageTitle";
import NotificationsSidebarMenu from "./NotificationSidebarMenu";

const Navbar = () => {
  const [pageTitle, _] = usePageTitle();

  return (
    <div className="w-full border-1 border-secondary p-3 items-center flex flex-row justify-between gap-2">
      <div className="flex flex-row gap-1 items-center">
        <SidebarTrigger size={"lg"}/>
        <h1>{pageTitle}</h1>
      </div>
      <div className="flex flex-row gap-1">
        {/* <Input className="w-[400px]" placeholder="Search..."/>
        <Button>
          <Search/>
        </Button> */}
      </div>
      <div className="flex flex-row items-center">
        <NotificationsSidebarMenu/>
      </div>
    </div>
  )
}

export default Navbar;