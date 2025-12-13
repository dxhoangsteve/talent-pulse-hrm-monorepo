import { API_CONFIG } from '../config/api.config';
import { authService } from './authService';
import { ApiResult } from '../types/types';

export interface Department {
  id: string;
  name: string;
  description?: string;
  managerId?: string;
  managerName?: string;
  deputyId?: string;
  deputyName?: string;
  userCount: number;
}

export interface UserOption {
  id: string;
  fullName: string;
  email?: string;
  currentDepartment?: string;
}

export interface UpdateLeadershipRequest {
  managerId?: string;
  deputyId?: string;
}

const getAuthHeaders = async () => {
  const token = await authService.getToken();
  return {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
  };
};

export const departmentService = {
  getDepartments: async (): Promise<Department[]> => {
    const token = await authService.getToken();
    const response = await fetch(`${API_CONFIG.API_URL}/Department`, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    const data = await response.json();
    if (data.isSuccessed) {
      return data.resultObj as Department[];
    }
    throw new Error(data.message || 'Failed to fetch departments');
  },

  createDepartment: async (deptData: { name: string; description: string; managerId: string; deputyId: string }) => {
    const token = await authService.getToken();
    const response = await fetch(`${API_CONFIG.API_URL}/Department`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(deptData),
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

  // Get available users for leadership selection
  getAvailableLeaders: async (): Promise<ApiResult<UserOption[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/Department/available-leaders`, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get available leaders error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách nhân viên' };
    }
  },

  // Update department leadership (Manager/Deputy)
  updateLeadership: async (departmentId: string, data: UpdateLeadershipRequest): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/Department/${departmentId}/leadership`, {
        method: 'PUT',
        headers,
        body: JSON.stringify(data),
      });
      return await response.json();
    } catch (error) {
      console.error('Update leadership error:', error);
      return { isSuccessed: false, message: 'Không thể cập nhật lãnh đạo phòng ban' };
    }
  },

  // Get employees in a department
  getDepartmentEmployees: async (departmentId: string): Promise<ApiResult<UserOption[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/Department/${departmentId}/employees`, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get department employees error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách nhân viên' };
    }
  },

  // Get all employees for assigning to department
  getAvailableEmployees: async (): Promise<ApiResult<UserOption[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/Account/users?pageSize=1000`, {
        method: 'GET',
        headers,
      });
      const data = await response.json();
      if (data.isSuccessed && data.resultObj && data.resultObj.items) {
        return { 
          isSuccessed: true, 
          resultObj: data.resultObj.items.map((u: any) => ({
            id: u.id,
            fullName: u.fullName,
            email: u.email,
            currentDepartment: u.departmentName
          }))
        };
      }
      return data;
    } catch (error) {
      console.error('Get available employees error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách nhân viên' };
    }
  },

  // Add employee to department
  addEmployeeToDepartment: async (departmentId: string, employeeId: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/Department/${departmentId}/employees`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ employeeId }),
      });
      return await response.json();
    } catch (error) {
      console.error('Add employee error:', error);
      return { isSuccessed: false, message: 'Không thể thêm nhân viên vào phòng ban' };
    }
  },

  // Remove employee from department
  removeEmployeeFromDepartment: async (departmentId: string, employeeId: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/Department/${departmentId}/employees/${employeeId}`, {
        method: 'DELETE',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Remove employee error:', error);
      return { isSuccessed: false, message: 'Không thể xóa nhân viên khỏi phòng ban' };
    }
  },
};
