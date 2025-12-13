using BaseSource.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.OvertimeRequest
{
    /// <summary>
    /// ViewModel để tạo đơn xin OT
    /// </summary>
    public class CreateOvertimeRequestVm
    {
        [Required(ErrorMessage = "Ngày OT là bắt buộc")]
        public DateTime OTDate { get; set; }

        [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc là bắt buộc")]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Hệ số OT (mặc định 1.5x)
        /// </summary>
        [Range(1, 3, ErrorMessage = "Hệ số OT phải từ 1 đến 3")]
        public decimal Multiplier { get; set; } = 1.5m;

        [MaxLength(500, ErrorMessage = "Lý do không được vượt quá 500 ký tự")]
        public string? Reason { get; set; }
    }

    /// <summary>
    /// ViewModel chi tiết đơn OT
    /// </summary>
    public class OvertimeRequestDetailVm
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public DateTime OTDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal Hours { get; set; }
        public decimal Multiplier { get; set; }
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
    /// ViewModel danh sách đơn OT
    /// </summary>
    public class OvertimeRequestListVm
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime OTDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal Hours { get; set; }
        public decimal Multiplier { get; set; }
        public RequestStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public string? ApprovedByName { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
