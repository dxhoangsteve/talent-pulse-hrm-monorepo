using BaseSource.ViewModels.Attendance;
using BaseSource.ViewModels.Common;

namespace BaseSource.Services.Services.Attendance
{
    public interface IAttendanceService
    {
        /// <summary>Check-in với GPS</summary>
        Task<ApiResult<AttendanceVm>> CheckInAsync(string userId, CheckInRequest request);
        
        /// <summary>Check-out với GPS</summary>
        Task<ApiResult<AttendanceVm>> CheckOutAsync(string userId, CheckOutRequest request);
        
        /// <summary>Lấy trạng thái check-in hôm nay</summary>
        Task<ApiResult<TodayAttendanceVm>> GetTodayStatusAsync(string userId);
        
        /// <summary>Lấy attendance của mình theo tháng</summary>
        Task<ApiResult<List<AttendanceVm>>> GetMyAttendanceAsync(string userId, int month, int year);
        
        /// <summary>Manager xem attendance phòng ban</summary>
        Task<ApiResult<List<AttendanceVm>>> GetDepartmentAttendanceAsync(string managerId, Guid departmentId, DateTime date);
        
        /// <summary>Admin xem tất cả attendance</summary>
        Task<ApiResult<List<AttendanceVm>>> GetAllAttendanceAsync(DateTime date);
    }
}
