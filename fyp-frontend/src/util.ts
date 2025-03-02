import { toast } from "sonner";
import Api from "./api";
import { SET_USER } from "./features/appSlice";
import AuthenticatedUser from "./models/AuthenticatedUser";
import { store } from "./store";

const EMAIL_STORAGE_KEY = "EMAIL_STORAGE_KEY";
const PASSWORD_STORAGE_KEY = "PASSWORD_STORAGE_KEY";

const Utils = {
  safelyRedirectToLoginPage: () => {
    if (!Utils.isCurrentLocationLoginPage()) {
      document.location.href = "/login";     
    }
  },
  isCurrentLocationLoginPage: () => {
    return document.location.href.split("/").at(-1) === "login";
  },
  saveLoginDetailsToLocalStorage: (email: string, password: string) => {
    localStorage.setItem(EMAIL_STORAGE_KEY, email);
    localStorage.setItem(PASSWORD_STORAGE_KEY, password);
  },
  clearLoginDetailsInLocalStorage: () => {
    localStorage.removeItem(EMAIL_STORAGE_KEY);
    localStorage.removeItem(PASSWORD_STORAGE_KEY);
  },
  loadLoginDetailsFromLocalStorage: () => {
    const email = localStorage.getItem(EMAIL_STORAGE_KEY) ?? "";
    const password = localStorage.getItem(PASSWORD_STORAGE_KEY) ?? "";
    if (email === null || password === null) return null;
    
    return { email: email, password: password};
  },
  relogin: async() => {
    var loginDetails = Utils.loadLoginDetailsFromLocalStorage();  
    if (loginDetails === null) {
      Utils.safelyRedirectToLoginPage();       
    }
    else {
      // login with details
      await Api.fetchLogin(loginDetails.email, loginDetails.password)
      .then((response) => {
        if (response.status === 200) {
          // successful login
          console.log("Successfully relogged in");
          const authUser: AuthenticatedUser = response.data.data as AuthenticatedUser;
          store.dispatch(SET_USER(authUser));       
          toast(`Welcome back ${authUser.displayName}! (successfully relogged in)`);
        }
      })
      .catch((error) => {
        // failed to relog
        store.dispatch(SET_USER(null));
        toast("Failed to re login, redirector to login page", { duration: 1000, onAutoClose: () => { Utils.safelyRedirectToLoginPage();}})
      })
    }
  } 
}

export default Utils;