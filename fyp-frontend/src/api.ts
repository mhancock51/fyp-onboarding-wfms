import axios from 'axios';
const ROUTE_URL = import.meta.env.VITE_BACKEND_SERVICE_ROUTE_URL;
const Api = {
  fetchLogin: async(emailAddress: string, password: string) => {
    return axios.post(`${ROUTE_URL}/auth/login?emailAddress=${emailAddress}&password=${password}`);
  }
}

export default Api;