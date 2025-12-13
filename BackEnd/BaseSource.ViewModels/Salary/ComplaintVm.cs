using BaseSource.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.Salary
{
    /// <summary>
    /// ViewModel để tạo khiếu nại lương
    /// </summary>
    public class CreateComplaintRequest
    {
        public int Month { get; set; }
        public int Year { get; set; }
        
        public ComplaintType ComplaintType { get; set; } = ComplaintType.NotPaid;
        
        [Required(ErrorMessage = "Vui lòng nhập nội dung khiếu nại")]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
        
        public string? SalarySlipId { get; set; }
    }

    /// <summary>
    /// ViewModel hiển thị khiếu nại
    /// </summary>
    public class ComplaintVm
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public string ComplaintTypeName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? ResolvedByName { get; set; }
        public string? Response { get; set; }
        public DateTime? ResolvedTime { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    /// <summary>
    /// Request để xử lý khiếu nại
    /// </summary>
    public class ResolveComplaintRequest
    {
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Resolved;
        
        [MaxLength(1000)]
        public string? Response { get; set; }
    }

    /// <summary>
    /// ViewModel để admin cập nhật lương
    /// </summary>
    public class UpdateSalaryRequest
    {
        public decimal? BaseSalary { get; set; }
        public decimal? OvertimePay { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Allowance { get; set; }
        public decimal? Deductions { get; set; }
        public string? Note { get; set; }
    }
}
