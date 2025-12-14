import { API_CONFIG } from '../config/api.config';
import { authService } from './authService';

export interface User {
  id: string;
  userName: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  roles: string[];
  isActive: boolean;
  employeeId?: string;
  departmentName?: string;
}

export interface PagedResult<T> {
  items: T[];
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export const userService = {
  getUsers: async (keyword: string = '', pageIndex: number = 1, pageSize: number = 10) => {
    const token = await authService.getToken();
    const queryParams = new URLSearchParams({
      keyword,
      pageIndex: pageIndex.toString(),
      pageSize: pageSize.toString(),
    });

    try {
      const response = await fetch(`${API_CONFIG.API_URL}/Account/users?${queryParams}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      const data = await response.json();
      if (data.isSuccessed) {
        return data.resultObj as PagedResult<User>;
      }
      throw new Error(data.message || 'Failed to fetch users');
    } catch (error) {
      console.error('Error fetching users:', error);
      throw error;
    }
  },

  createUser: async (userData: any) => {
    // using existing Register endpoint, but usually admin would use a different one or Register with specific role logic
    // For now assuming Register is public or we use it here. 
    // Wait, Register updates "Employee" role by default. Admin might want to set roles.
    // Keeping it simple as per request to just "Add".
    const response = await fetch(`${API_CONFIG.API_URL}/Account/Register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(userData),
    });
    const data = await response.json();
    if (!data.isSuccessed) {
      if (data.validationErrors && data.validationErrors.length > 0) {
        const errors = data.validationErrors.map((e: any) => e.error).join('\n');
        throw new Error(errors);
      }
      throw new Error(data.message || 'Có lỗi xảy ra');
    }
    return data;
  },

  updateUser: async (id: string, userData: any) => {
    const token = await authService.getToken();
    const response = await fetch(`${API_CONFIG.API_URL}/Account/users/${id}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(userData),
    });
    const data = await response.json();
    if (!data.isSuccessed) throw new Error(data.message);
    return data;
  },

  deleteUser: async (id: string) => {
    const token = await authService.getToken();
    const response = await fetch(`${API_CONFIG.API_URL}/Account/users/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    });
    const data = await response.json();
    if (!data.isSuccessed) throw new Error(data.message);
    return data;
  },

  setPassword: async (id: string, newPassword: string) => {
    const token = await authService.getToken();
    const response = await fetch(`${API_CONFIG.API_URL}/Account/users/${id}/set-password`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(newPassword), // Sending string as body
    });
    const data = await response.json();
    if (!data.isSuccessed) throw new Error(data.message);
    return data;
  }
};
