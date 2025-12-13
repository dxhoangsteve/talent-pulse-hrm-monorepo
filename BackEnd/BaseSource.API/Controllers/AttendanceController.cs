using BaseSource.Services.Services.Attendance;
using BaseSource.ViewModels.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.Controllers
{
    [Route("api/attendance")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Check-in với GPS
        /// </summary>
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            var result = await _attendanceService.CheckInAsync(UserId, request);
            return Ok(result);
        }

        /// <summary>
        /// Check-out với GPS
        /// </summary>
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            var result = await _attendanceService.CheckOutAsync(UserId, request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy trạng thái check-in hôm nay
        /// </summary>
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayStatus()
        {
            var result = await _attendanceService.GetTodayStatusAsync(UserId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy attendance của mình theo tháng
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAttendance([FromQuery] int month, [FromQuery] int year)
        {
            var result = await _attendanceService.GetMyAttendanceAsync(UserId, month, year);
            return Ok(result);
        }

        /// <summary>
        /// Manager xem attendance phòng ban
        /// </summary>
        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,DeputyManager")]
        public async Task<IActionResult> GetDepartmentAttendance(Guid departmentId, [FromQuery] DateTime date)
        {
            var result = await _attendanceService.GetDepartmentAttendanceAsync(UserId, departmentId, date);
            return Ok(result);
        }

        /// <summary>
        /// Admin xem tất cả attendance
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin,HR")]
        public async Task<IActionResult> GetAllAttendance([FromQuery] DateTime date)
        {
            var result = await _attendanceService.GetAllAttendanceAsync(date);
            return Ok(result);
        }
    }
}
