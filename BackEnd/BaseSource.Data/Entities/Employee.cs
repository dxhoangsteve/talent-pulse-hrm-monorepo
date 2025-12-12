using System;
using System.Collections.Generic;

namespace BaseSource.Data.Entities
{
    public class Employee
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string DepartmentId { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public DateTime JoinDate { get; set; }
        public string? UserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public Department Department { get; set; } = null!;
        public AppUser? User { get; set; }
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public ICollection<OvertimeRequest> OvertimeRequests { get; set; } = new List<OvertimeRequest>();
    }
}
