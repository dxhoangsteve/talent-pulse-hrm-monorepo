using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.LeaveRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using BaseSource.Shared.Helpers;

namespace BaseSource.Services.Services.LeaveRequest
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly BaseSourceDbContext _context;
        private readonly ILogger<LeaveRequestService> _logger;

        public LeaveRequestService(BaseSourceDbContext context, ILogger<LeaveRequestService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResult<LeaveRequestDetailVm>> CreateAsync(string userId, CreateLeaveRequestVm model)
        {
            try
            {
                // Validate dates
                if (model.EndDate < model.StartDate)
                {
                    return new ApiErrorResult<LeaveRequestDetailVm>("Ngày kết thúc phải sau ngày bắt đầu");
                }

                // Get user to find their EmployeeId
                var user = await _context.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user?.Employee == null)
                {
                    return new ApiErrorResult<LeaveRequestDetailVm>("Không tìm thấy thông tin nhân viên");
                }

                // Calculate total days (excluding weekends)
                var totalDays = CalculateBusinessDays(model.StartDate, model.EndDate);

                var leaveRequest = new Data.Entities.LeaveRequest
                {
                    EmployeeId = user.Employee.Id,
                    LeaveType = model.LeaveType,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    TotalDays = totalDays,
                    Reason = model.Reason,
                    Status = RequestStatus.Pending,
                    CreatedTime = TimeHelper.VietnamNow
                };

                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();

                return await GetByIdAsync(leaveRequest.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request for user {UserId}", userId);
                return new ApiErrorResult<LeaveRequestDetailVm>("Tạo đơn nghỉ phép thất bại");
            }
        }

        public async Task<ApiResult<LeaveRequestDetailVm>> GetByIdAsync(string id)
        {
            var request = await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.ApprovedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return new ApiErrorResult<LeaveRequestDetailVm>("Không tìm thấy đơn nghỉ phép");
            }

            var vm = MapToDetailVm(request);
            return new ApiSuccessResult<LeaveRequestDetailVm>(vm);
        }

        public async Task<ApiResult<List<LeaveRequestListVm>>> GetMyRequestsAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Employee == null)
            {
                return new ApiErrorResult<List<LeaveRequestListVm>>("Không tìm thấy thông tin nhân viên");
            }

            var requests = await _context.LeaveRequests
                .Include(r => r.Employee)
                .Include(r => r.ApprovedByUser)
                .Where(r => r.EmployeeId == user.Employee.Id)
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();

            var result = requests.Select(MapToListVm).ToList();
            return new ApiSuccessResult<List<LeaveRequestListVm>>(result);
        }

        public async Task<ApiResult<List<LeaveRequestListVm>>> GetPendingForApprovalAsync(string approverId)
        {
            var approver = await _context.Users
                .Include(u => u.Department)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == approverId);

            if (approver == null)
            {
                return new ApiErrorResult<List<LeaveRequestListVm>>("Không tìm thấy thông tin người duyệt");
            }

            var roles = approver.UserRoles.Select(ur => ur.Role.Name).ToList();
            var isAdmin = roles.Any(r => r == "SuperAdmin" || r == "Admin" || r == "HR");
            var isManager = approver.Position == PositionType.Manager || approver.Position == PositionType.DeputyManager;

            IQueryable<Data.Entities.LeaveRequest> query = _context.LeaveRequests
                .Include(r => r.Employee)
                    .ThenInclude(e => e.User)
                .Include(r => r.ApprovedByUser)
                .Where(r => r.Status == RequestStatus.Pending);

            // If not admin, filter by department
            if (!isAdmin && isManager && approver.DepartmentId.HasValue)
            {
                query = query.Where(r => r.Employee.User != null && 
                                         r.Employee.User.DepartmentId == approver.DepartmentId);
            }
            else if (!isAdmin && !isManager)
            {
                // Regular employee cannot approve
                return new ApiSuccessResult<List<LeaveRequestListVm>>(new List<LeaveRequestListVm>());
            }

            var requests = await query
                .OrderByDescending(r => r.CreatedTime)
                .ToListAsync();

            var result = requests.Select(MapToListVm).ToList();
            return new ApiSuccessResult<List<LeaveRequestListVm>>(result);
        }

        public async Task<ApiResult<PagedResult<LeaveRequestListVm>>> GetAllAsync(Guid? departmentId, RequestStatus? status, int page, int pageSize)
        {
            try
            {
                var query = _context.LeaveRequests
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.User)
                            .ThenInclude(u => u.Department)
                    .Include(r => r.ApprovedByUser)
                    .AsQueryable();

                // Filter by department if specified
                if (departmentId.HasValue)
                {
                    query = query.Where(r => r.Employee.User != null && r.Employee.User.DepartmentId == departmentId);
                }

                // Filter by status if specified
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

                var pagedResult = new PagedResult<LeaveRequestListVm>
                {
                    Items = items,
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return new ApiSuccessResult<PagedResult<LeaveRequestListVm>>(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all leave requests");
                return new ApiErrorResult<PagedResult<LeaveRequestListVm>>("Lấy danh sách đơn nghỉ phép thất bại");
            }
        }


        public async Task<ApiResult<bool>> ApproveAsync(string requestId, string approverId)
        {
            try
            {
                var request = await _context.LeaveRequests
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.User)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy đơn nghỉ phép");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return new ApiErrorResult<bool>("Đơn đã được xử lý trước đó");
                }

                // Check approval permission
                var canApprove = await CanApproveRequest(approverId, request);
                if (!canApprove)
                {
                    return new ApiErrorResult<bool>("Bạn không có quyền duyệt đơn này");
                }

                request.Status = RequestStatus.Approved;
                request.ApprovedBy = approverId;
                request.ApprovedTime = TimeHelper.VietnamNow;
                request.UpdatedTime = TimeHelper.VietnamNow;

                await _context.SaveChangesAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request {RequestId}", requestId);
                return new ApiErrorResult<bool>("Duyệt đơn thất bại");
            }
        }

        public async Task<ApiResult<bool>> RejectAsync(string requestId, string approverId, string? reason)
        {
            try
            {
                var request = await _context.LeaveRequests
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.User)
                    .FirstOrDefaultAsync(r => r.Id == requestId);

                if (request == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy đơn nghỉ phép");
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
                request.ApprovedTime = TimeHelper.VietnamNow;
                request.RejectReason = reason;
                request.UpdatedTime = TimeHelper.VietnamNow;

                await _context.SaveChangesAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting leave request {RequestId}", requestId);
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

                var request = await _context.LeaveRequests
                    .FirstOrDefaultAsync(r => r.Id == requestId && r.EmployeeId == user.Employee.Id);

                if (request == null)
                {
                    return new ApiErrorResult<bool>("Không tìm thấy đơn nghỉ phép");
                }

                if (request.Status != RequestStatus.Pending)
                {
                    return new ApiErrorResult<bool>("Chỉ có thể hủy đơn đang chờ duyệt");
                }

                request.Status = RequestStatus.Cancelled;
                request.UpdatedTime = TimeHelper.VietnamNow;

                await _context.SaveChangesAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request {RequestId}", requestId);
                return new ApiErrorResult<bool>("Hủy đơn thất bại");
            }
        }

        #region Private Methods

        private async Task<bool> CanApproveRequest(string approverId, Data.Entities.LeaveRequest request)
        {
            var approver = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == approverId);

            if (approver == null) return false;

            var roles = approver.UserRoles.Select(ur => ur.Role.Name).ToList();
            
            // Admin/SuperAdmin/HR can approve all
            if (roles.Any(r => r == "SuperAdmin" || r == "Admin" || r == "HR"))
            {
                return true;
            }

            // Manager/Deputy can approve within their department
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

        private static decimal CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            // Count all calendar days (inclusive of start and end date)
            // Example: 13-14 = 2 days (ngày 13 và ngày 14)
            var totalDays = (endDate.Date - startDate.Date).Days + 1;
            return Math.Max(1, totalDays); // At least 1 day
        }

        private static LeaveRequestDetailVm MapToDetailVm(Data.Entities.LeaveRequest request)
        {
            return new LeaveRequestDetailVm
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                EmployeeName = request.Employee?.FullName ?? "N/A",
                DepartmentName = request.Employee?.User?.Department?.Name,
                LeaveType = request.LeaveType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalDays = request.TotalDays,
                Reason = request.Reason,
                Status = request.Status,
                ApprovedBy = request.ApprovedBy,
                ApprovedByName = request.ApprovedByUser?.FullName,
                ApprovedTime = request.ApprovedTime,
                RejectReason = request.RejectReason,
                CreatedTime = request.CreatedTime
            };
        }

        private static LeaveRequestListVm MapToListVm(Data.Entities.LeaveRequest request)
        {
            return new LeaveRequestListVm
            {
                Id = request.Id,
                EmployeeName = request.Employee?.FullName ?? "N/A",
                LeaveType = request.LeaveType,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalDays = request.TotalDays,
                Status = request.Status,
                ApprovedByName = request.ApprovedByUser?.FullName,
                CreatedTime = request.CreatedTime
            };
        }

        #endregion
    }
}
