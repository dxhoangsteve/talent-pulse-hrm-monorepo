import axios, { AxiosError, InternalAxiosRequestConfig, AxiosResponse } from 'axios';
import { authService } from './authService';
import { API_BASE_URL } from '../constants/config';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add auth token to requests
apiClient.interceptors.request.use(
  async (config: InternalAxiosRequestConfig) => {
    const token = await authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// Handle response errors
apiClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (error: AxiosError) => {
    if (error.response?.status === 401) {
      await authService.removeToken();
    }
    return Promise.reject(error);
  }
);

export default apiClient;

