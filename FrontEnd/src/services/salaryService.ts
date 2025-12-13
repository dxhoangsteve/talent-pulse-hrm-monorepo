import apiClient from './apiClient';
import { ApiResult } from '../types/types';

export interface SalaryVm {
  id: string;
  employeeId: string;
  employeeName: string;
  departmentName?: string;
  month: number;
  year: number;
  workDays: number;
  actualWorkDays: number;
  lateDays: number;
  earlyLeaveDays: number;
  baseSalary: number;
  overtimePay: number;
  bonus: number;
  allowance: number;
  deductions: number;
  insurance: number;
  tax: number;
  netSalary: number;
  status: string;
  statusName: string;
  approvedByName?: string;
  approvedTime?: string;
  paidByName?: string;
  paidTime?: string;
  note?: string;
  createdTime: string;
}

export const salaryService = {
  getMySalary: async (month?: number, year?: number): Promise<ApiResult<SalaryVm[]>> => {
    let url = '/salary/my';
    const params = [];
    if (month) params.push(`month=${month}`);
    if (year) params.push(`year=${year}`);
    if (params.length > 0) url += '?' + params.join('&');
    
    const response = await apiClient.get(url);
    return response.data;
  },

  getLatestPaidSalary: async (): Promise<SalaryVm | null> => {
    try {
      const now = new Date();
      const result = await salaryService.getMySalary(now.getMonth() + 1, now.getFullYear());
      if (result.isSuccessed && result.resultObj) {
        return result.resultObj.find(s => s.status === 'Paid') || null;
      }
      return null;
    } catch {
      return null;
    }
  },

  getDepartmentSalary: async (month: number, year: number): Promise<ApiResult<SalaryVm[]>> => {
    const response = await apiClient.get(`/salary/department?month=${month}&year=${year}`);
    return response.data;
  },

  getAllSalary: async (month: number, year: number): Promise<ApiResult<SalaryVm[]>> => {
    const response = await apiClient.get(`/salary?month=${month}&year=${year}`);
    return response.data;
  },
};
