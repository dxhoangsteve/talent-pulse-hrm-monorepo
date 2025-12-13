using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.LeaveRequest;

namespace BaseSource.Services.Services.LeaveRequest
{
    public interface ILeaveRequestService
    {
        /// <summary>
        /// Tạo đơn xin nghỉ phép mới
        /// </summary>
        Task<ApiResult<LeaveRequestDetailVm>> CreateAsync(string userId, CreateLeaveRequestVm model);

        /// <summary>
        /// Lấy chi tiết đơn nghỉ phép
        /// </summary>
        Task<ApiResult<LeaveRequestDetailVm>> GetByIdAsync(string id);

        /// <summary>
        /// Lấy danh sách đơn nghỉ phép của user hiện tại
        /// </summary>
        Task<ApiResult<List<LeaveRequestListVm>>> GetMyRequestsAsync(string userId);

        /// <summary>
        /// Lấy danh sách đơn chờ duyệt (cho Admin/Manager/Deputy)
        /// </summary>
        Task<ApiResult<List<LeaveRequestListVm>>> GetPendingForApprovalAsync(string approverId);

        /// <summary>
        /// Lấy tất cả đơn nghỉ phép - Admin (với filter phòng ban, status, paging)
        /// </summary>
        Task<ApiResult<PagedResult<LeaveRequestListVm>>> GetAllAsync(Guid? departmentId, BaseSource.Shared.Enums.RequestStatus? status, int page, int pageSize);

        /// <summary>
        /// Duyệt đơn nghỉ phép
        /// </summary>
        Task<ApiResult<bool>> ApproveAsync(string requestId, string approverId);

        /// <summary>
        /// Từ chối đơn nghỉ phép
        /// </summary>
        Task<ApiResult<bool>> RejectAsync(string requestId, string approverId, string? reason);

        /// <summary>
        /// Hủy đơn nghỉ phép (chỉ người tạo có thể hủy)
        /// </summary>
        Task<ApiResult<bool>> CancelAsync(string requestId, string userId);
    }
}
