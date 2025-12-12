using System;

namespace BaseSource.Data.Entities
{
    public class OvertimeRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EmployeeId { get; set; } = string.Empty;
        public DateTime OTDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal Hours { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        // Navigation
        public Employee Employee { get; set; } = null!;
    }
}
