using BaseSource.Shared.Enums;
using System;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Đơn xin nghỉ phép
    /// </summary>
    public class LeaveRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string EmployeeId { get; set; } = string.Empty;
        
        /// <summary>Loại nghỉ phép</summary>
        public LeaveType LeaveType { get; set; }
        
        /// <summary>Ngày bắt đầu</summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>Ngày kết thúc</summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>Tổng số ngày nghỉ</summary>
        public decimal TotalDays { get; set; }
        
        /// <summary>Lý do nghỉ</summary>
        public string? Reason { get; set; }
        
        /// <summary>Trạng thái</summary>
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        
        /// <summary>Người duyệt</summary>
        public string? ApprovedBy { get; set; }
        
        public DateTime? ApprovedTime { get; set; }
        public string? RejectReason { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;
        
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ApprovedBy")]
        public virtual AppUser? ApprovedByUser { get; set; }
    }
}
