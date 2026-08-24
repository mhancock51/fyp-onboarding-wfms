// App-level constants derived from environment variables.
// Change VITE_APP_NAME / VITE_EMAIL_DOMAIN in your .env file to rebrand.

export const APP_NAME: string = import.meta.env.VITE_APP_NAME || "FlowPath"
export const APP_EMAIL_DOMAIN: string = import.meta.env.VITE_EMAIL_DOMAIN || "flowpath.com"

export const APP_SALES_EMAIL = `sales@${APP_EMAIL_DOMAIN}`
export const APP_SUPPORT_EMAIL = `support@${APP_EMAIL_DOMAIN}`
