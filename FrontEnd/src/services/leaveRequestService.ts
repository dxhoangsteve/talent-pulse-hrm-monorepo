import { API_CONFIG } from '../config/api.config';
import { authService } from './authService';
import { CreateLeaveRequest, LeaveRequestItem, ApiResult } from '../types/types';

const getAuthHeaders = async () => {
  const token = await authService.getToken();
  return {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`,
  };
};

export const leaveRequestService = {
  // Create new leave request
  createRequest: async (data: CreateLeaveRequest): Promise<ApiResult<LeaveRequestItem>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/leave-requests`, {
        method: 'POST',
        headers,
        body: JSON.stringify(data),
      });
      return await response.json();
    } catch (error) {
      console.error('Create leave request error:', error);
      return { isSuccessed: false, message: 'Không thể tạo đơn nghỉ phép' };
    }
  },

  // Get my leave requests
  getMyRequests: async (): Promise<ApiResult<LeaveRequestItem[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/leave-requests/my`, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get my leave requests error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách đơn nghỉ phép' };
    }
  },

  // Get pending requests for approval
  getPendingForApproval: async (): Promise<ApiResult<LeaveRequestItem[]>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/leave-requests/pending`, {
        method: 'GET',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Get pending leave requests error:', error);
      return { isSuccessed: false, message: 'Không thể tải danh sách đơn chờ duyệt' };
    }
  },

  // Approve request
  approveRequest: async (id: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/leave-requests/${id}/approve`, {
        method: 'POST',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Approve leave request error:', error);
      return { isSuccessed: false, message: 'Không thể duyệt đơn' };
    }
  },

  // Reject request
  rejectRequest: async (id: string, reason?: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/leave-requests/${id}/reject`, {
        method: 'POST',
        headers,
        body: JSON.stringify({ requestId: id, rejectReason: reason }),
      });
      return await response.json();
    } catch (error) {
      console.error('Reject leave request error:', error);
      return { isSuccessed: false, message: 'Không thể từ chối đơn' };
    }
  },

  // Cancel my request
  cancelRequest: async (id: string): Promise<ApiResult<boolean>> => {
    try {
      const headers = await getAuthHeaders();
      const response = await fetch(`${API_CONFIG.API_URL}/leave-requests/${id}`, {
        method: 'DELETE',
        headers,
      });
      return await response.json();
    } catch (error) {
      console.error('Cancel leave request error:', error);
      return { isSuccessed: false, message: 'Không thể hủy đơn' };
    }
  },
};
