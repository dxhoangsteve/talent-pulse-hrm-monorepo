namespace BaseSource.ViewModels.Attendance
{
    /// <summary>
    /// Request cho check-in/check-out
    /// </summary>
    public class CheckInRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public bool IsMockedLocation { get; set; }
    }

    public class CheckOutRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public bool IsMockedLocation { get; set; }
    }

    /// <summary>
    /// View model chi tiết attendance
    /// </summary>
    public class AttendanceVm
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public double? CheckInAccuracy { get; set; }
        
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        public double? CheckOutAccuracy { get; set; }
        
        public bool IsMockedLocation { get; set; }
        
        public string Status { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        
        public decimal WorkHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Trạng thái check-in hôm nay
    /// </summary>
    public class TodayAttendanceVm
    {
        public bool HasCheckedIn { get; set; }
        public bool HasCheckedOut { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public decimal WorkHours { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
