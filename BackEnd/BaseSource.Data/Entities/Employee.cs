using BaseSource.Shared.Enums;
using System;
using System.Collections.Generic;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Thông tin nhân viên
    /// </summary>
    public class Employee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        /// <summary>Mã nhân viên (unique)</summary>
        public string EmployeeCode { get; set; } = string.Empty;
        
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        
        /// <summary>Ngày sinh</summary>
        public DateTime? DateOfBirth { get; set; }
        
        /// <summary>Giới tính</summary>
        public Gender Gender { get; set; } = Gender.Male;
        
        /// <summary>Địa chỉ</summary>
        public string? Address { get; set; }
        
        /// <summary>Cấp bậc</summary>
        public EmployeeLevel Level { get; set; } = EmployeeLevel.Fresher;
        
        /// <summary>Trạng thái làm việc</summary>
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        
        /// <summary>Chức danh</summary>
        public string? Position { get; set; }
        
        /// <summary>Phòng ban</summary>
        /// <summary>Phòng ban</summary>
        public Guid? DepartmentId { get; set; }
        
        /// <summary>Lương cơ bản</summary>
        public decimal BaseSalary { get; set; }
        
        /// <summary>Số ngày phép năm</summary>
        public int AnnualLeaveDays { get; set; } = 12;
        
        /// <summary>Số ngày phép còn lại</summary>
        public decimal RemainingLeaveDays { get; set; } = 12;
        
        /// <summary>Ngày vào công ty</summary>
        public DateTime JoinDate { get; set; }
        
        /// <summary>Ngày kết thúc thử việc</summary>
        public DateTime? ProbationEndDate { get; set; }
        
        /// <summary>Liên kết với tài khoản (nullable)</summary>
        public string? UserId { get; set; }
        
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public Department Department { get; set; } = null!;
        public AppUser? User { get; set; }
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public ICollection<OvertimeRequest> OvertimeRequests { get; set; } = new List<OvertimeRequest>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
