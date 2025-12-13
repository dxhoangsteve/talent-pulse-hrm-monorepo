using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Attendance;
using BaseSource.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace BaseSource.Services.Services.Attendance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly BaseSourceDbContext _context;
        
        // Vietnam timezone: UTC+7
        private static readonly TimeSpan VietnamOffset = TimeSpan.FromHours(7);
        private static DateTime VietnamNow => DateTime.UtcNow.Add(VietnamOffset);

        public AttendanceService(BaseSourceDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<AttendanceVm>> CheckInAsync(string userId, CheckInRequest request)
        {
            try
            {
                // Get employee from user
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                if (employee == null)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };
                }

                // Reject mocked location
                if (request.IsMockedLocation)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Không thể check-in với vị trí giả (mocked location)" };
                }

                var vietnamNow = VietnamNow;
                var today = vietnamNow.Date;

                // Check if already checked in today
                var existingAttendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == today);

                if (existingAttendance != null && existingAttendance.CheckInTime.HasValue)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Bạn đã check-in hôm nay rồi" };
                }

                var checkInTime = vietnamNow.TimeOfDay;
                var standardStartTime = new TimeSpan(8, 0, 0); // 8:00 AM

                // Determine status
                var status = AttendanceStatus.Present;
                if (checkInTime > standardStartTime.Add(TimeSpan.FromMinutes(15)))
                {
                    status = AttendanceStatus.Late;
                }

                var attendance = existingAttendance ?? new Data.Entities.Attendance
                {
                    EmployeeId = employee.Id,
                    Date = today,
                };

                attendance.CheckInTime = checkInTime;
                attendance.CheckInLatitude = request.Latitude;
                attendance.CheckInLongitude = request.Longitude;
                attendance.CheckInAccuracy = request.Accuracy;
                attendance.IsMockedLocation = request.IsMockedLocation;
                attendance.Status = status;
                attendance.UpdatedTime = vietnamNow;

                if (existingAttendance == null)
                {
                    _context.Attendances.Add(attendance);
                }

                await _context.SaveChangesAsync();

                return new ApiResult<AttendanceVm>
                {
                    IsSuccessed = true,
                    ResultObj = MapToVm(attendance, employee.FullName)
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<AttendanceVm>> CheckOutAsync(string userId, CheckOutRequest request)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                if (employee == null)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };
                }

                var vietnamNow = VietnamNow;
                var today = vietnamNow.Date;
                var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == today);

                if (attendance == null || !attendance.CheckInTime.HasValue)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Bạn chưa check-in hôm nay" };
                }

                if (attendance.CheckOutTime.HasValue)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Bạn đã check-out hôm nay rồi" };
                }

                // Reject mocked location
                if (request.IsMockedLocation)
                {
                    return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = "Không thể check-out với vị trí giả (mocked location)" };
                }

                var checkOutTime = vietnamNow.TimeOfDay;
                var standardEndTime = new TimeSpan(17, 0, 0); // 5:00 PM

                attendance.CheckOutTime = checkOutTime;
                attendance.CheckOutLatitude = request.Latitude;
                attendance.CheckOutLongitude = request.Longitude;
                attendance.CheckOutAccuracy = request.Accuracy;
                
                if (request.IsMockedLocation)
                    attendance.IsMockedLocation = true;

                // Calculate work hours
                var workHours = (checkOutTime - attendance.CheckInTime.Value).TotalHours;
                attendance.WorkHours = (decimal)Math.Max(0, workHours);

                // Check early leave
                if (checkOutTime < standardEndTime.Subtract(TimeSpan.FromMinutes(15)))
                {
                    if (attendance.Status == AttendanceStatus.Late)
                    {
                        attendance.Status = AttendanceStatus.HalfDay; // Both late and early
                    }
                    else
                    {
                        attendance.Status = AttendanceStatus.EarlyLeave;
                    }
                }

                // Calculate OT hours
                if (checkOutTime > standardEndTime)
                {
                    var otHours = (checkOutTime - standardEndTime).TotalHours;
                    attendance.OvertimeHours = (decimal)otHours;
                }

                attendance.UpdatedTime = vietnamNow;
                await _context.SaveChangesAsync();

                return new ApiResult<AttendanceVm>
                {
                    IsSuccessed = true,
                    ResultObj = MapToVm(attendance, employee.FullName)
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<AttendanceVm> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<TodayAttendanceVm>> GetTodayStatusAsync(string userId)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                if (employee == null)
                {
                    return new ApiResult<TodayAttendanceVm> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };
                }

                var today = VietnamNow.Date;
                var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == today);

                var result = new TodayAttendanceVm
                {
                    HasCheckedIn = attendance?.CheckInTime.HasValue ?? false,
                    HasCheckedOut = attendance?.CheckOutTime.HasValue ?? false,
                    CheckInTime = attendance?.CheckInTime,
                    CheckOutTime = attendance?.CheckOutTime,
                    WorkHours = attendance?.WorkHours ?? 0,
                    Status = attendance?.Status.ToString() ?? "NotCheckedIn"
                };

                return new ApiResult<TodayAttendanceVm> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<TodayAttendanceVm> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<AttendanceVm>>> GetMyAttendanceAsync(string userId, int month, int year)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                if (employee == null)
                {
                    return new ApiResult<List<AttendanceVm>> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };
                }

                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                var attendances = await _context.Attendances
                    .Where(a => a.EmployeeId == employee.Id && a.Date >= startDate && a.Date < endDate)
                    .OrderByDescending(a => a.Date)
                    .ToListAsync();

                var result = attendances.Select(a => MapToVm(a, employee.FullName)).ToList();

                return new ApiResult<List<AttendanceVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<AttendanceVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<AttendanceVm>>> GetDepartmentAttendanceAsync(string managerId, Guid departmentId, DateTime date)
        {
            try
            {
                var attendances = await _context.Attendances
                    .Include(a => a.Employee)
                    .Where(a => a.Employee.DepartmentId == departmentId && a.Date == date.Date)
                    .OrderBy(a => a.Employee.FullName)
                    .ToListAsync();

                var result = attendances.Select(a => MapToVm(a, a.Employee.FullName)).ToList();

                return new ApiResult<List<AttendanceVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<AttendanceVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<AttendanceVm>>> GetAllAttendanceAsync(DateTime date)
        {
            try
            {
                var attendances = await _context.Attendances
                    .Include(a => a.Employee)
                    .Where(a => a.Date == date.Date)
                    .OrderBy(a => a.Employee.FullName)
                    .ToListAsync();

                var result = attendances.Select(a => MapToVm(a, a.Employee.FullName)).ToList();

                return new ApiResult<List<AttendanceVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<AttendanceVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        private AttendanceVm MapToVm(Data.Entities.Attendance attendance, string employeeName)
        {
            return new AttendanceVm
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                EmployeeName = employeeName,
                Date = attendance.Date,
                CheckInTime = attendance.CheckInTime,
                CheckOutTime = attendance.CheckOutTime,
                CheckInLatitude = attendance.CheckInLatitude,
                CheckInLongitude = attendance.CheckInLongitude,
                CheckInAccuracy = attendance.CheckInAccuracy,
                CheckOutLatitude = attendance.CheckOutLatitude,
                CheckOutLongitude = attendance.CheckOutLongitude,
                CheckOutAccuracy = attendance.CheckOutAccuracy,
                IsMockedLocation = attendance.IsMockedLocation,
                Status = attendance.Status.ToString(),
                StatusName = GetStatusName(attendance.Status),
                WorkHours = attendance.WorkHours,
                OvertimeHours = attendance.OvertimeHours,
                Note = attendance.Note
            };
        }

        private string GetStatusName(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.Present => "Có mặt",
                AttendanceStatus.Absent => "Vắng mặt",
                AttendanceStatus.Late => "Đi muộn",
                AttendanceStatus.EarlyLeave => "Về sớm",
                AttendanceStatus.HalfDay => "Nửa ngày",
                AttendanceStatus.OnLeave => "Nghỉ phép",
                AttendanceStatus.Holiday => "Nghỉ lễ",
                AttendanceStatus.WorkFromHome => "Làm từ xa",
                _ => "N/A"
            };
        }
    }
}
