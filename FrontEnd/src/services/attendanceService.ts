import apiClient from './apiClient';
import { ApiResult } from '../types/types';

export interface CheckInRequest {
  latitude: number;
  longitude: number;
  accuracy: number;
  isMockedLocation: boolean;
}

export interface TodayAttendanceVm {
  hasCheckedIn: boolean;
  hasCheckedOut: boolean;
  checkInTime?: string;
  checkOutTime?: string;
  workHours: number;
  status: string;
}

export interface AttendanceVm {
  id: string;
  employeeId: string;
  employeeName: string;
  date: string;
  checkInTime?: string;
  checkOutTime?: string;
  checkInLatitude?: number;
  checkInLongitude?: number;
  checkInAccuracy?: number;
  checkOutLatitude?: number;
  checkOutLongitude?: number;
  checkOutAccuracy?: number;
  isMockedLocation: boolean;
  status: string;
  statusName: string;
  workHours: number;
  overtimeHours: number;
  note?: string;
}

export const attendanceService = {
  checkIn: async (request: CheckInRequest): Promise<ApiResult<AttendanceVm>> => {
    const response = await apiClient.post('/attendance/check-in', request);
    return response.data;
  },

  checkOut: async (request: CheckInRequest): Promise<ApiResult<AttendanceVm>> => {
    const response = await apiClient.post('/attendance/check-out', request);
    return response.data;
  },

  getTodayStatus: async (): Promise<ApiResult<TodayAttendanceVm>> => {
    const response = await apiClient.get('/attendance/today');
    return response.data;
  },

  getMyAttendance: async (month: number, year: number): Promise<ApiResult<AttendanceVm[]>> => {
    const response = await apiClient.get(`/attendance/my?month=${month}&year=${year}`);
    return response.data;
  },

  getDepartmentAttendance: async (departmentId: string, date: string): Promise<ApiResult<AttendanceVm[]>> => {
    const response = await apiClient.get(`/attendance/department/${departmentId}?date=${date}`);
    return response.data;
  },

  getAllAttendance: async (date: string): Promise<ApiResult<AttendanceVm[]>> => {
    const response = await apiClient.get(`/attendance?date=${date}`);
    return response.data;
  },
};
