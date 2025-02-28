import axios, { AxiosResponse } from 'axios';
import { store } from './store';
const ROUTE_URL = import.meta.env.VITE_BACKEND_SERVICE_ROUTE_URL;

const AuthInstance = axios.create();
AuthInstance.interceptors.request.use((config: any) => {
  const token = store.getState().app.user?.jwtToken;
  config.headers.Authorization = `Bearer ${token}`;
  return config
})

const Api = {
  fetchLogin: async(emailAddress: string, password: string) => {
    return axios.post(`${ROUTE_URL}/auth/login?emailAddress=${emailAddress}&password=${password}`);
  },
  testTokenValidity: async() => {
    return AuthInstance.get(`${ROUTE_URL}/auth/test`);
  }
}

export default Api;