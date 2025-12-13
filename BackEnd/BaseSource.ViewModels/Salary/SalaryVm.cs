namespace BaseSource.ViewModels.Salary
{
    /// <summary>
    /// View model phiếu lương
    /// </summary>
    public class SalaryVm
    {
        public string Id { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        
        public int Month { get; set; }
        public int Year { get; set; }
        
        public int WorkDays { get; set; }
        public decimal ActualWorkDays { get; set; }
        public int LateDays { get; set; }
        public int EarlyLeaveDays { get; set; }
        
        public decimal BaseSalary { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Bonus { get; set; }
        public decimal Allowance { get; set; }
        public decimal Deductions { get; set; }
        public decimal Insurance { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
        
        public string Status { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string? PaidByName { get; set; }
        public DateTime? PaidTime { get; set; }
        
        public string? Note { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    /// <summary>
    /// Request tạo/tính lương
    /// </summary>
    public class CalculateSalaryRequest
    {
        public string EmployeeId { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? Deductions { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Request duyệt/phát lương
    /// </summary>
    public class PaySalaryRequest
    {
        public string SalaryId { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    /// <summary>
    /// Query lịch sử phát lương
    /// </summary>
    public class SalaryHistoryQuery
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? EmployeeId { get; set; }
    }
}
