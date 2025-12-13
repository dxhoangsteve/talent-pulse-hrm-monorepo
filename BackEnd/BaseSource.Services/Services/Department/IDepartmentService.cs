using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Department;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseSource.Services.Services.Department
{
    public interface IDepartmentService
    {
        Task<ApiResult<bool>> CreateAsync(CreateDepartmentRequest request);
        Task<ApiResult<List<DepartmentVm>>> GetAllAsync();
        
        /// <summary>
        /// Cập nhật trưởng/phó phòng (chỉ Admin được phép)
        /// </summary>
        Task<ApiResult<bool>> UpdateLeadershipAsync(Guid departmentId, UpdateDepartmentLeadershipVm model);
        
        /// <summary>
        /// Lấy danh sách user có thể làm trưởng/phó phòng
        /// </summary>
        Task<ApiResult<List<UserSelectVm>>> GetAvailableLeadersAsync();

        /// <summary>
        /// Lấy phòng ban mà user đang quản lý (Manager/Deputy)
        /// Trả về null nếu user không quản lý phòng ban nào
        /// </summary>
        Task<Guid?> GetManagedDepartmentIdAsync(string userId);

        /// <summary>
        /// Lấy danh sách nhân viên trong phòng ban
        /// </summary>
        Task<ApiResult<List<UserSelectVm>>> GetDepartmentEmployeesAsync(Guid departmentId);

        /// <summary>
        /// Thêm nhân viên vào phòng ban
        /// </summary>
        Task<ApiResult<bool>> AddEmployeeToDepartmentAsync(Guid departmentId, string employeeId);

        /// <summary>
        /// Xóa nhân viên khỏi phòng ban
        /// </summary>
        Task<ApiResult<bool>> RemoveEmployeeFromDepartmentAsync(Guid departmentId, string employeeId);
    }

    /// <summary>
    /// ViewModel để hiển thị user trong dropdown chọn trưởng/phó phòng
    /// </summary>
    public class UserSelectVm
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CurrentDepartment { get; set; }
    }
}

