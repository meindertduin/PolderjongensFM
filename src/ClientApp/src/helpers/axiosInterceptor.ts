import axios, {AxiosRequestConfig} from 'axios';
import store from '@/store/index';

export default function initInterceptor() {
    axios.interceptors.request.use((config: AxiosRequestConfig) => {
        const token:string = store.state.oidcStore.access_token; 
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
            config.withCredentials = true;
        }
        return config;
    }, (err) => {
        return Promise.reject(err);
    });
}