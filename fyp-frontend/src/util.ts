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
  }
}

export default Utils;