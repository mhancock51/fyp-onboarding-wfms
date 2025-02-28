const Utils = {
  safelyRedirectToLoginPage: () => {
    if (!Utils.isCurrentLocationLoginPage()) {
      document.location.href = "/login";     
    }
  },
  isCurrentLocationLoginPage: () => {
    return document.location.href.split("/").at(-1) === "login";
  }
}

export default Utils;