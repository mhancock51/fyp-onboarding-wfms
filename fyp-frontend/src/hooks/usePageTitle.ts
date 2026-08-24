import { SET_PAGE_TITLE } from "@/features/appSlice";
import { RootState } from "@/store";
import { useCallback } from "react";
import { useDispatch, useSelector } from "react-redux";

export function usePageTitle(): [string, (title: string) => void] {
  const dispatch = useDispatch();
  const pageTitle = useSelector((state: RootState) => state.app.pageTitle)


  const setPageTitle = useCallback(
    (title: string) => {
      dispatch(SET_PAGE_TITLE(title))
    },
    [dispatch]
  )
  return [pageTitle, setPageTitle];
}