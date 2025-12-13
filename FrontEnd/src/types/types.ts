export type UserRole = 'SuperAdmin' | 'Admin' | 'HR' | 'Manager' | 'Employee';

export type RootStackParamList = {
  Login: undefined;
  AdminDashboard: undefined;
  UserManagement: undefined;
  EmployeeDashboard: undefined;
  LeaveRequest: undefined;
  OTRequest: undefined;
  ApprovalScreen: undefined;
  DepartmentManagement: undefined;
  ManagerDashboard: undefined;
  DepartmentEmployees: undefined;
  DepartmentSalary: undefined;
  AttendanceScreen: undefined;
  AttendanceHistory: undefined;
  MySalary: undefined;
  SalaryManagement: undefined;
  LeaveHistory: undefined;
  OTHistory: undefined;
  SalaryHistory: undefined;
};

export interface DecodedToken {
  email: string;
  role: string | string[]; // "SuperAdmin;Admin" or single role
  name: string;
  nameid: string; // user id
  exp: number;
}

// Enums
export enum RequestStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Cancelled = 3,
}

export enum SalaryStatus {
  Draft = 0,
  Pending = 1,
  Approved = 2,
  Paid = 3,
  Cancelled = 4,
}

export enum LeaveType {
  Annual = 0,
  Sick = 1,
  Unpaid = 2,
  Maternity = 3,
  Paternity = 4,
  Bereavement = 5,
  Wedding = 6,
  Compensatory = 7,
  Other = 99,
}

// Leave Request
export interface CreateLeaveRequest {
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
  reason?: string;
}

export interface LeaveRequestItem {
  id: string;
  employeeName: string;
  leaveType: LeaveType;
  leaveTypeName: string;
  startDate: string;
  endDate: string;
  totalDays: number;
  status: RequestStatus;
  statusName: string;
  approvedByName?: string;
  createdTime: string;
}

// OT Request
export interface CreateOTRequest {
  otDate: string;
  startTime: string;
  endTime: string;
  multiplier: number;
  reason?: string;
}

export interface OTRequestItem {
  id: string;
  employeeName: string;
  otDate: string;
  startTime: string;
  endTime: string;
  hours: number;
  multiplier: number;
  status: RequestStatus;
  statusName: string;
  approvedByName?: string;
  createdTime: string;
}

// API Response wrapper
export interface ApiResult<T> {
  isSuccessed: boolean;
  resultObj?: T;
  message?: string;
}

// Helper functions
export const getStatusLabel = (status: RequestStatus): string => {
  switch (status) {
    case RequestStatus.Pending: return 'Chờ duyệt';
    case RequestStatus.Approved: return 'Đã duyệt';
    case RequestStatus.Rejected: return 'Từ chối';
    case RequestStatus.Cancelled: return 'Đã hủy';
    default: return 'N/A';
  }
};

export const getLeaveTypeLabel = (type: LeaveType): string => {
  switch (type) {
    case LeaveType.Annual: return 'Nghỉ phép năm';
    case LeaveType.Sick: return 'Nghỉ ốm';
    case LeaveType.Unpaid: return 'Nghỉ không lương';
    case LeaveType.Maternity: return 'Nghỉ thai sản (nữ)';
    case LeaveType.Paternity: return 'Nghỉ thai sản (nam)';
    case LeaveType.Bereavement: return 'Nghỉ tang';
    case LeaveType.Wedding: return 'Nghỉ cưới';
    case LeaveType.Compensatory: return 'Nghỉ bù';
    case LeaveType.Other: return 'Khác';
    default: return 'N/A';
  }
};

