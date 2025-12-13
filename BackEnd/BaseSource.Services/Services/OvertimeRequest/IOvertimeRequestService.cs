using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.OvertimeRequest;

namespace BaseSource.Services.Services.OvertimeRequest
{
    public interface IOvertimeRequestService
    {
        /// <summary>
        /// Tạo đơn xin OT mới
        /// </summary>
        Task<ApiResult<OvertimeRequestDetailVm>> CreateAsync(string userId, CreateOvertimeRequestVm model);

        /// <summary>
        /// Lấy chi tiết đơn OT
        /// </summary>
        Task<ApiResult<OvertimeRequestDetailVm>> GetByIdAsync(string id);

        /// <summary>
        /// Lấy danh sách đơn OT của user hiện tại
        /// </summary>
        Task<ApiResult<List<OvertimeRequestListVm>>> GetMyRequestsAsync(string userId);

        /// <summary>
        /// Lấy danh sách đơn chờ duyệt (cho Admin/Manager/Deputy)
        /// </summary>
        Task<ApiResult<List<OvertimeRequestListVm>>> GetPendingForApprovalAsync(string approverId);

        /// <summary>
        /// Lấy tất cả đơn OT - Admin (với filter phòng ban, status, paging)
        /// </summary>
        Task<ApiResult<PagedResult<OvertimeRequestListVm>>> GetAllAsync(Guid? departmentId, BaseSource.Shared.Enums.RequestStatus? status, int page, int pageSize);

        /// <summary>
        /// Duyệt đơn OT
        /// </summary>
        Task<ApiResult<bool>> ApproveAsync(string requestId, string approverId);

        /// <summary>
        /// Từ chối đơn OT
        /// </summary>
        Task<ApiResult<bool>> RejectAsync(string requestId, string approverId, string? reason);

        /// <summary>
        /// Hủy đơn OT (chỉ người tạo có thể hủy)
        /// </summary>
        Task<ApiResult<bool>> CancelAsync(string requestId, string userId);
    }
}
