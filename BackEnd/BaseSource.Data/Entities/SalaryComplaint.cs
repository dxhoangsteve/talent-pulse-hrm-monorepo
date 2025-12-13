using BaseSource.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSource.Data.Entities
{
    /// <summary>
    /// Khiếu nại về lương khi nhân viên chưa nhận được lương
    /// </summary>
    public class SalaryComplaint
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [ForeignKey(nameof(EmployeeId))]
        public virtual AppUser? Employee { get; set; }

        /// <summary>
        /// Tham chiếu đến phiếu lương (nếu có)
        /// </summary>
        public Guid? SalarySlipId { get; set; }

        [ForeignKey(nameof(SalarySlipId))]
        public virtual Salary? SalarySlip { get; set; }

        /// <summary>
        /// Tháng khiếu nại
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Năm khiếu nại
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Loại khiếu nại
        /// </summary>
        public ComplaintType ComplaintType { get; set; } = ComplaintType.NotPaid;

        /// <summary>
        /// Nội dung khiếu nại
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái xử lý
        /// </summary>
        public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;

        /// <summary>
        /// Người xử lý
        /// </summary>
        public string? ResolvedById { get; set; }

        [ForeignKey(nameof(ResolvedById))]
        public virtual AppUser? ResolvedBy { get; set; }

        /// <summary>
        /// Nội dung phản hồi
        /// </summary>
        [MaxLength(1000)]
        public string? Response { get; set; }

        /// <summary>
        /// Thời gian xử lý
        /// </summary>
        public DateTime? ResolvedTime { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
