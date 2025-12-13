using BaseSource.Shared.Enums;
using System;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Chấm công hàng ngày
    /// </summary>
    public class Attendance
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string EmployeeId { get; set; } = string.Empty;
        
        /// <summary>Ngày chấm công</summary>
        public DateTime Date { get; set; }
        
        /// <summary>Giờ check-in</summary>
        public TimeSpan? CheckInTime { get; set; }
        
        /// <summary>Giờ check-out</summary>
        public TimeSpan? CheckOutTime { get; set; }
        
        /// <summary>Trạng thái</summary>
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        
        /// <summary>Số giờ làm việc</summary>
        public decimal WorkHours { get; set; }
        
        /// <summary>Số giờ OT</summary>
        public decimal OvertimeHours { get; set; }
        
        /// <summary>Ghi chú</summary>
        public string? Note { get; set; }
        
        /// <summary>Địa điểm check-in (IP/GPS)</summary>
        public string? CheckInLocation { get; set; }
        
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;
    }
}
