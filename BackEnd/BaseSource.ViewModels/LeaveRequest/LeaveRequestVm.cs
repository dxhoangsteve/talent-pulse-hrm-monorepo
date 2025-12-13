using BaseSource.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.LeaveRequest
{
    /// <summary>
    /// ViewModel để tạo đơn xin nghỉ phép
    /// </summary>
    public class CreateLeaveRequestVm
    {
        [Required(ErrorMessage = "Loại nghỉ phép là bắt buộc")]
        public LeaveType LeaveType { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        [MaxLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string? Reason { get; set; }
    }

    /// <summary>
    /// ViewModel chi tiết đơn nghỉ phép
    /// </summary>
    public class LeaveRequestDetailVm
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public LeaveType LeaveType { get; set; }
        public string LeaveTypeName => LeaveType.ToString();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public string? Reason { get; set; }
        public RequestStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string? RejectReason { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    /// <summary>
    /// ViewModel danh sách đơn nghỉ phép
    /// </summary>
    public class LeaveRequestListVm
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public LeaveType LeaveType { get; set; }
        public string LeaveTypeName => LeaveType.ToString();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDays { get; set; }
        public RequestStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? ApprovedByName { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
