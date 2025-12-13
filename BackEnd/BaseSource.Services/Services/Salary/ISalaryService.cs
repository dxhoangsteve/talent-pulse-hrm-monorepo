using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Salary;

namespace BaseSource.Services.Services.Salary
{
    public interface ISalaryService
    {
        /// <summary>Tính lương cho nhân viên (Admin)</summary>
        Task<ApiResult<SalaryVm>> CalculateSalaryAsync(string adminId, CalculateSalaryRequest request);
        
        /// <summary>Lấy phiếu lương của mình</summary>
        Task<ApiResult<List<SalaryVm>>> GetMySalaryAsync(string userId, int? month, int? year);
        
        /// <summary>Lấy tất cả phiếu lương (Admin)</summary>
        Task<ApiResult<List<SalaryVm>>> GetAllSalaryAsync(int month, int year);

        /// <summary>Lấy phiếu lương theo phòng ban (Manager/Deputy)</summary>
        Task<ApiResult<List<SalaryVm>>> GetDepartmentSalaryAsync(Guid departmentId, int month, int year);
        
        /// <summary>Duyệt phiếu lương (Admin)</summary>
        Task<ApiResult<bool>> ApproveSalaryAsync(string adminId, string salaryId);
        
        /// <summary>Phát lương (Admin)</summary>
        Task<ApiResult<bool>> PaySalaryAsync(string adminId, string salaryId, string? note);
        
        /// <summary>Xem lịch sử phát lương</summary>
        Task<ApiResult<List<SalaryVm>>> GetPaymentHistoryAsync(SalaryHistoryQuery query);
    }
}

