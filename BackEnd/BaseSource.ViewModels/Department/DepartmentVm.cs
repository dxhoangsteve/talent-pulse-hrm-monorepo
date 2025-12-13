using System;
using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.Department
{
    public class DepartmentVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string? DeputyId { get; set; }
        public string DeputyName { get; set; }
        public int UserCount { get; set; }
    }

    /// <summary>
    /// ViewModel để cập nhật trưởng/phó phòng ban
    /// </summary>
    public class UpdateDepartmentLeadershipVm
    {
        /// <summary>
        /// ID của Trưởng phòng mới (có thể null để xóa)
        /// </summary>
        public string? ManagerId { get; set; }

        /// <summary>
        /// ID của Phó phòng mới (có thể null để xóa)
        /// </summary>
        public string? DeputyId { get; set; }
    }

    /// <summary>
    /// ViewModel để tạo phòng ban mới
    /// </summary>
    public class CreateDepartmentVm
    {
        [Required(ErrorMessage = "Tên phòng ban là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Tên phòng ban không được vượt quá 255 ký tự")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        public string? ManagerId { get; set; }
        public string? DeputyId { get; set; }
    }
}
