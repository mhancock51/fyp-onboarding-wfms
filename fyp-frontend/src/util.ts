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
  isCurrentLocationRegisterPage: () => {
    return document.location.href.split("/").at(-1)?.startsWith("register");
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
  },
  downloadFile(byteString: string, filename: string) {
    // convert byte string to a Blob
    const byteCharacters = atob(byteString);
    const byteNumbers = new Uint8Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
      byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const blob = new Blob([byteNumbers], { type: "application/octet-stream" });
    // create a URL for the blob
    const url = URL.createObjectURL(blob);

    // create an anchor element and trigger download
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();

    // Cleanup
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  },
  convertFileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = (error) => reject(error);
    });
  }
}

export default Utils;