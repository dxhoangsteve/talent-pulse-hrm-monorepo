using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Salary;

namespace BaseSource.Services.Services.Salary
{
    public interface ISalaryService
    {
        /// <summary>Tính lương cho nhân viên (Admin)</summary>
        Task<ApiResult<SalaryVm>> CalculateSalaryAsync(string adminId, CalculateSalaryRequest request);

        /// <summary>Xem trước bảng tính lương (Admin) - Không lưu Database</summary>
        Task<ApiResult<SalaryVm>> CalculateSalaryPreviewAsync(string adminId, CalculateSalaryRequest request);

        /// <summary>Lấy phiếu lương của mình</summary>
        Task<ApiResult<List<SalaryVm>>> GetMySalaryAsync(string userId, int? month, int? year);

        /// <summary>Lấy tất cả phiếu lương (Admin) với filter phòng ban và paging</summary>
        Task<ApiResult<PagedResult<SalaryVm>>> GetAllSalaryAsync(int month, int year, Guid? departmentId = null, int page = 1, int pageSize = 20);

        /// <summary>Lấy phiếu lương theo phòng ban (Manager/Deputy)</summary>
        Task<ApiResult<List<SalaryVm>>> GetDepartmentSalaryAsync(Guid departmentId, int month, int year);

        /// <summary>Duyệt phiếu lương (Admin)</summary>
        Task<ApiResult<bool>> ApproveSalaryAsync(string adminId, string salaryId);

        /// <summary>Phát lương (Admin)</summary>
        Task<ApiResult<bool>> PaySalaryAsync(string adminId, string salaryId, string? note);

        /// <summary>Xem lịch sử phát lương</summary>
        Task<ApiResult<List<SalaryVm>>> GetPaymentHistoryAsync(SalaryHistoryQuery query);

        /// <summary>Cập nhật thông tin lương (Admin)</summary>
        Task<ApiResult<bool>> UpdateSalaryAsync(string adminId, string salaryId, UpdateSalaryRequest request);

        // Complaint methods
        /// <summary>Tạo khiếu nại lương (Employee)</summary>
        Task<ApiResult<ComplaintVm>> CreateComplaintAsync(string employeeId, CreateComplaintRequest request);

        /// <summary>Lấy khiếu nại của mình (Employee)</summary>
        Task<ApiResult<List<ComplaintVm>>> GetMyComplaintsAsync(string employeeId);

        /// <summary>Lấy tất cả khiếu nại (Admin)</summary>
        Task<ApiResult<List<ComplaintVm>>> GetAllComplaintsAsync();

        /// <summary>Xác nhận lương (Employee) - Đồng thuận hoặc Khiếu nại</summary>
        Task<ApiResult<bool>> ConfirmSalaryAsync(string userId, string salaryId, bool isConfirmed, string? note);

        /// <summary>Xử lý khiếu nại (Admin)</summary>
        Task<ApiResult<bool>> ResolveComplaintAsync(string adminId, Guid complaintId, ResolveComplaintRequest request);
    }
}
