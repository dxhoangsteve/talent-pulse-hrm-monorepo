using BaseSource.Shared.Enums;
using System;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Bảng lương tháng
    /// </summary>
    public class Salary
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string EmployeeId { get; set; } = string.Empty;
        
        /// <summary>Tháng</summary>
        public int Month { get; set; }
        
        /// <summary>Năm</summary>
        public int Year { get; set; }
        
        /// <summary>Số ngày làm việc (chuẩn)</summary>
        public int WorkDays { get; set; }
        
        /// <summary>Số ngày thực tế làm việc</summary>
        public decimal ActualWorkDays { get; set; }
        
        /// <summary>Số ngày đi muộn</summary>
        public int LateDays { get; set; }
        
        /// <summary>Số ngày về sớm</summary>
        public int EarlyLeaveDays { get; set; }
        
        /// <summary>Lương cơ bản</summary>
        public decimal BaseSalary { get; set; }
        
        /// <summary>Lương OT</summary>
        public decimal OvertimePay { get; set; }
        
        /// <summary>Thưởng</summary>
        public decimal Bonus { get; set; }
        
        /// <summary>Phụ cấp</summary>
        public decimal Allowance { get; set; }
        
        /// <summary>Khấu trừ</summary>
        public decimal Deductions { get; set; }
        
        /// <summary>Bảo hiểm</summary>
        public decimal Insurance { get; set; }
        
        /// <summary>Thuế TNCN</summary>
        public decimal Tax { get; set; }
        
        /// <summary>Lương thực nhận</summary>
        public decimal NetSalary { get; set; }
        
        /// <summary>Trạng thái</summary>
        public SalaryStatus Status { get; set; } = SalaryStatus.Draft;
        
        /// <summary>Ghi chú</summary>
        public string? Note { get; set; }
        
        /// <summary>ID người duyệt</summary>
        public string? ApprovedBy { get; set; }
        
        /// <summary>Thời gian duyệt</summary>
        public DateTime? ApprovedTime { get; set; }
        
        /// <summary>ID người phát lương</summary>
        public string? PaidBy { get; set; }
        
        /// <summary>Thời gian phát lương</summary>
        public DateTime? PaidTime { get; set; }
        
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedTime { get; set; }

        // Navigation
        public Employee Employee { get; set; } = null!;
        public AppUser? ApprovedByUser { get; set; }
        public AppUser? PaidByUser { get; set; }
    }
}

