import { API_CONFIG } from '../config/api.config';
import { authService } from './authService';
import { CreateOTRequest, OTRequestItem, ApiResult } from '../types/types';

const getAuthHeaders = async () => {
  const token = await authService.getToken();
  return {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
  };
};

export const overtimeRequestService = {
  // Create new OT request
  createRequest: async (data: CreateOTRequest): Promise<ApiResult<OTRequestItem>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/overtime-requests`, {
        method: 'POST',
        headers,
        body: JSON.stringify(data),
      });
      return await response.json();
    } catch (error) {
      console.error('Create OT request error:', error);
      return { isSuccessed: false, message: 'Không thể tạo đơn OT' };
    }
  },

  // Get my OT requests
  getMyRequests: async (): Promise<ApiResult<OTRequestItem[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/overtime-requests/my`, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get my OT requests error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách đơn OT' };
    }
  },

  // Get all OT requests (Admin only) with filters
  getAllRequests: async (params?: {
    departmentId?: string;
    status?: number;
    page?: number;
    pageSize?: number;
  }): Promise<ApiResult<{ items: OTRequestItem[]; totalCount: number; pageIndex: number; pageSize: number; totalPages: number }>> => {
    try {
      const headers = await getAuthHeaders();
      const queryParams = new URLSearchParams();
      if (params?.departmentId) queryParams.append('departmentId', params.departmentId);
      if (params?.status !== undefined) queryParams.append('status', params.status.toString());
      if (params?.page) queryParams.append('page', params.page.toString());
      if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
      
      const url = `${API_CONFIG.API_URL}/overtime-requests/all${queryParams.toString() ? `?${queryParams}` : ''}`;
      const response = await fetch(url, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get all OT requests error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách đơn OT' };
    }
  },

  // Get pending requests for approval
  getPendingForApproval: async (): Promise<ApiResult<OTRequestItem[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/overtime-requests/pending`, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get pending OT requests error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách đơn OT chờ duyệt' };
    }
  },

  // Approve request
  approveRequest: async (id: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/overtime-requests/${id}/approve`, {
        method: 'POST',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Approve OT request error:', error);
      return { isSuccessed: false, message: 'Không thể duyệt đơn OT' };
    }
  },

  // Reject request
  rejectRequest: async (id: string, reason?: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/overtime-requests/${id}/reject`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ requestId: id, rejectReason: reason }),
      });
      return await response.json();
    } catch (error) {
      console.error('Reject OT request error:', error);
      return { isSuccessed: false, message: 'Không thể từ chối đơn OT' };
    }
  },

  // Cancel my request
  cancelRequest: async (id: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/overtime-requests/${id}`, {
        method: 'DELETE',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Cancel OT request error:', error);
      return { isSuccessed: false, message: 'Không thể hủy đơn OT' };
    }
  },
};
