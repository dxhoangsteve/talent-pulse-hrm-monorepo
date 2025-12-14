
import { API_CONFIG } from '../config/api.config';
import { authService } from './authService';
import { ApiResult } from '../types/types';

export interface AdminDashboardStats {
    totalUsers: number;
    totalDepartments: number;
    pendingLeaveRequests: number;
    totalSalaryPaidThisMonth: number;
}

export const dashboardService = {
    getAdminStats: async (): Promise<ApiResult<AdminDashboardStats>> => {
        try {
            const token = await authService.getToken();
            const response = await fetch(`${API_CONFIG.API_URL}/Dashboard/admin-stats`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json',
                },
            });
            return await response.json();
        } catch (error) {
            console.error('Get admin stats error:', error);
            return { isSuccessed: false, message: 'Could not fetch stats' };
        }
    }
};
