// API Configuration for TalentPulse
// Thay đổi IP này thành IP của máy chạy Backend

// Lấy IP laptop: chạy `ip route get 1 | awk '{print $7; exit}'`
const LOCAL_IP = '192.168.1.13';
const API_PORT = '5294'; // HTTP port từ launchSettings.json

export const API_CONFIG = {
  BASE_URL: `http://${LOCAL_IP}:${API_PORT}`,
  API_URL: `http://${LOCAL_IP}:${API_PORT}/api`,
  TIMEOUT: 30000, // 30 seconds
};

// API Endpoints
export const ENDPOINTS = {
  // Auth
  LOGIN: '/Account/Authenticate',
  REGISTER: '/Account/Register',
  FORGOT_PASSWORD: '/Account/ForgotPassword',
  RESET_PASSWORD: '/Account/ResetPassword',
  CONFIRM_EMAIL: '/Account/ConfirmEmail',
  CHANGE_PASSWORD: '/Account/ChangePassword',
  
  // User
  GET_USER_INFO: '/Account/GetUserInfo',
  GET_PROFILE: '/Account/profile',
  UPDATE_PROFILE: '/Account/update',
};

// Helper function để gọi API
export async function apiRequest<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<{ success: boolean; data?: T; message?: string }> {
  try {
    const response = await fetch(`${API_CONFIG.API_URL}${endpoint}`, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers,
      },
      ...options,
    });

    const result = await response.json();
    
    // API trả về format: { isSucceeded, resultObj, message }
    if (result.isSucceeded) {
      return { success: true, data: result.resultObj };
    }
    return { success: false, message: result.message || 'Có lỗi xảy ra' };
  } catch (error) {
    console.error('API Error:', error);
    return { success: false, message: 'Không thể kết nối đến server' };
  }
}
