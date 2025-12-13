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
        
        // Check-in GPS Location
        /// <summary>Latitude check-in</summary>
        public double? CheckInLatitude { get; set; }
        
        /// <summary>Longitude check-in</summary>
        public double? CheckInLongitude { get; set; }
        
        /// <summary>GPS Accuracy check-in (meters)</summary>
        public double? CheckInAccuracy { get; set; }
        
        // Check-out GPS Location
        /// <summary>Latitude check-out</summary>
        public double? CheckOutLatitude { get; set; }
        
        /// <summary>Longitude check-out</summary>
        public double? CheckOutLongitude { get; set; }
        
        /// <summary>GPS Accuracy check-out (meters)</summary>
        public double? CheckOutAccuracy { get; set; }
        
        /// <summary>Phát hiện vị trí giả (mocked)</summary>
        public bool IsMockedLocation { get; set; } = false;
        
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;
    }
}

