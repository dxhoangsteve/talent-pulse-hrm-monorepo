using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.OvertimeRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSource.Services.Services.OvertimeRequest
{
    public class OvertimeRequestService : IOvertimeRequestService
    {
        private readonly BaseSourceDbContext _context;
        private readonly ILogger<OvertimeRequestService> _logger;

        public OvertimeRequestService(BaseSourceDbContext context, ILogger<OvertimeRequestService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResult<OvertimeRequestDetailVm>> CreateAsync(string userId, CreateOvertimeRequestVm model)
        {
            try
            {
                // Validate times
                if (model.EndTime <= model.StartTime)
                {
                    return new ApiErrorResult<OvertimeRequestDetailVm>("Giờ kết thúc phải sau giờ bắt đầu");
                }

                var user = await _context.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user?.Employee == null)
                {
                    return new ApiErrorResult<OvertimeRequestDetailVm>("Không tìm thấy thông tin nhân viên");
                }

                // Calculate hours
                var hours = (decimal)(model.EndTime - model.StartTime).TotalHours;

                var otRequest = new Data.Entities.OvertimeRequest
                {
                    EmployeeId = user.Employee.Id,
                    OTDate = model.OTDate,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    Hours = hours,
                    Multiplier = model.Multiplier,
                    Reason = model.Reason,
                    Status = RequestStatus.Pending,
                    CreatedTime = DateTime.UtcNow
                };

                _context.OvertimeRequests.Add(otRequest);
                await _context.SaveChangesAsync();

                return await GetByIdAsync(otRequest.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating OT request for user {UserId}", userId);
                return new ApiErrorResult<OvertimeRequestDetailVm>("Tạo đơn OT thất bại");
            }
        }

        public async Task<ApiResult<OvertimeRequestDetailVm>> GetByIdAsync(string id)
        {
            var request = await _context.OvertimeRequests
                .Include(r => r.Employee)
                .Include(r => r.ApprovedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return new ApiErrorResult<OvertimeRequestDetailVm>("Không tìm thấy đơn OT");
            }

            var vm = MapToDetailVm(request);
            return new ApiSuccessResult<OvertimeRequestDetailVm>(vm);
        }

        public async Task<ApiResult<List<OvertimeRequestListVm>>> GetMyRequestsAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Employee == null)
            {
                return new ApiErrorResult<List<OvertimeRequestListVm>>("Không tìm thấy thông tin nhân viên");
            }

            var requests = await _context.OvertimeRequests
                .Include(r => r.Employee)
                .Include(r => r.ApprovedByUser)
                .Where(r => r.EmployeeId == user.Employee.Id)
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();

            var result = requests.Select(MapToListVm).ToList();
            return new ApiSuccessResult<List<OvertimeRequestListVm>>(result);
        }

        public async Task<ApiResult<List<OvertimeRequestListVm>>> GetPendingForApprovalAsync(string approverId)
        {
            var approver = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == approverId);

            if (approver == null)
            {
                return new ApiErrorResult<List<OvertimeRequestListVm>>("Không tìm thấy thông tin người duyệt");
            }

            var roles = approver.UserRoles.Select(ur => ur.Role.Name).ToList();
            var isAdmin = roles.Any(r => r == "SuperAdmin" || r == "Admin" || r == "HR");
            var isManager = approver.Position == PositionType.Manager || approver.Position == PositionType.DeputyManager;

            IQueryable<Data.Entities.OvertimeRequest> query = _context.OvertimeRequests
                .Include(r => r.Employee)
                    .ThenInclude(e => e.User)
                .Include(r => r.ApprovedByUser)
                .Where(r => r.Status == RequestStatus.Pending);

            if (!isAdmin && isManager && approver.DepartmentId.HasValue)
            {
                query = query.Where(r => r.Employee.User != null && 
                                         r.Employee.User.DepartmentId == approver.DepartmentId);
            }
            else if (!isAdmin && !isManager)
            {
                return new ApiSuccessResult<List<OvertimeRequestListVm>>(new List<OvertimeRequestListVm>());
            }

            var requests = await query
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();

            var result = requests.Select(MapToListVm).ToList();
            return new ApiSuccessResult<List<OvertimeRequestListVm>>(result);
        }

        public async Task<ApiResult<PagedResult<OvertimeRequestListVm>>> GetAllAsync(Guid? departmentId, RequestStatus? status, int page, int pageSize)
        {
            try
            {
                var query = _context.OvertimeRequests
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.User)
                            .ThenInclude(u => u.Department)
                    .Include(r => r.ApprovedByUser)
                    .AsQueryable();

                if (departmentId.HasValue)
                {
                    query = query.Where(r => r.Employee.User != null && r.Employee.User.DepartmentId == departmentId);
                }

                if (status.HasValue)
                {
                    query = query.Where(r => r.Status == status.Value);
                }

                var totalCount = await query.CountAsync();

                var requests = await query
                    .OrderByDescending(r => r.CreatedTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = requests.Select(MapToListVm).ToList();

                var pagedResult = new PagedResult<OvertimeRequestListVm>
                {
                    Items = items,
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return new ApiSuccessResult<PagedResult<OvertimeRequestListVm>>(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all OT requests");
                return new ApiErrorResult<PagedResult<OvertimeRequestListVm>>("Lấy danh sách đơn OT thất bại");
            }
        }

        public async Task<ApiResult<bool>> ApproveAsync(string requestId, string approverId)
        {
            try
            {
                var request = await _context.OvertimeRequests
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.User)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy đơn OT");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return new ApiErrorResult<bool>("Đơn đã được xử lý trước đó");
                }

                var canApprove = await CanApproveRequest(approverId, request);
                if (!canApprove)
                {
                    return new ApiErrorResult<bool>("Bạn không có quyền duyệt đơn này");
                }

                request.Status = RequestStatus.Approved;
                request.ApprovedBy = approverId;
                request.ApprovedTime = DateTime.UtcNow;
                request.UpdatedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving OT request {RequestId}", requestId);
                return new ApiErrorResult<bool>("Duyệt đơn thất bại");
            }
        }

        public async Task<ApiResult<bool>> RejectAsync(string requestId, string approverId, string? reason)
        {
            try
            {
                var request = await _context.OvertimeRequests
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.User)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy đơn OT");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return new ApiErrorResult<bool>("Đơn đã được xử lý trước đó");
                }

                var canApprove = await CanApproveRequest(approverId, request);
                if (!canApprove)
                {
                    return new ApiErrorResult<bool>("Bạn không có quyền từ chối đơn này");
                }

                request.Status = RequestStatus.Rejected;
                request.ApprovedBy = approverId;
                request.ApprovedTime = DateTime.UtcNow;
                request.RejectReason = reason;
                request.UpdatedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting OT request {RequestId}", requestId);
                return new ApiErrorResult<bool>("Từ chối đơn thất bại");
            }
        }

        public async Task<ApiResult<bool>> CancelAsync(string requestId, string userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user?.Employee == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy thông tin nhân viên");
                }

                var request = await _context.OvertimeRequests
                    .FirstOrDefaultAsync(r => r.Id == requestId && r.EmployeeId == user.Employee.Id);

                if (request == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy đơn OT");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return new ApiErrorResult<bool>("Chỉ có thể hủy đơn đang chờ duyệt");
                }

                request.Status = RequestStatus.Cancelled;
                request.UpdatedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling OT request {RequestId}", requestId);
                return new ApiErrorResult<bool>("Hủy đơn thất bại");
            }
        }

        #region Private Methods

        private async Task<bool> CanApproveRequest(string approverId, Data.Entities.OvertimeRequest request)
        {
            var approver = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == approverId);

            if (approver == null) return false;

            var roles = approver.UserRoles.Select(ur => ur.Role.Name).ToList();
            
            if (roles.Any(r => r == "SuperAdmin" || r == "Admin" || r == "HR"))
            {
                return true;
            }

            if (approver.Position == PositionType.Manager || approver.Position == PositionType.DeputyManager)
            {
                var employeeUser = request.Employee?.User ?? 
                    await _context.Users.FirstOrDefaultAsync(u => u.Employee != null && u.Employee.Id == request.EmployeeId);
                
                if (employeeUser != null && approver.DepartmentId.HasValue && 
                    employeeUser.DepartmentId == approver.DepartmentId)
                {
                    return true;
                }
            }

            return false;
        }

        private static OvertimeRequestDetailVm MapToDetailVm(Data.Entities.OvertimeRequest request)
        {
            return new OvertimeRequestDetailVm
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                EmployeeName = request.Employee?.FullName ?? "N/A",
                DepartmentName = request.Employee?.User?.Department?.Name,
                OTDate = request.OTDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Hours = request.Hours,
                Multiplier = request.Multiplier,
                Reason = request.Reason,
                Status = request.Status,
                ApprovedBy = request.ApprovedBy,
                ApprovedByName = request.ApprovedByUser?.FullName,
                ApprovedTime = request.ApprovedTime,
                RejectReason = request.RejectReason,
                CreatedTime = request.CreatedTime
            };
        }

        private static OvertimeRequestListVm MapToListVm(Data.Entities.OvertimeRequest request)
        {
            return new OvertimeRequestListVm
            {
                Id = request.Id,
                EmployeeName = request.Employee?.FullName ?? "N/A",
                OTDate = request.OTDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Hours = request.Hours,
                Multiplier = request.Multiplier,
                Status = request.Status,
                ApprovedByName = request.ApprovedByUser?.FullName,
                CreatedTime = request.CreatedTime
            };
        }

        #endregion
    }
}
