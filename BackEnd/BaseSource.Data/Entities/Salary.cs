using System;

namespace BaseSource.Data.Entities
{
    public class Salary
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EmployeeId { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public string Status { get; set; } = "Draft"; // Draft, Approved, Paid
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedTime { get; set; }
        public string? ApprovedBy { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;
    }
}
