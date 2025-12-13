using BaseSouce.Services.Services.User;
using BaseSource.Services.Services.Department;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDepartmentService _departmentService;

        public UserController(IUserService userService, IDepartmentService departmentService)
        {
            _userService = userService;
            _departmentService = departmentService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Lấy danh sách user (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetUsersPaging([FromQuery] GetUserPagingRequest request)
        {
            var result = await _userService.GetUsersPagingAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo phòng ban (Manager/Deputy xem phòng ban mình)
        /// </summary>
        [HttpGet("department")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,DeputyManager")]
        public async Task<IActionResult> GetDepartmentUsers()
        {
            // Get the department this user manages
            var managedDeptId = await _departmentService.GetManagedDepartmentIdAsync(UserId);
            
            if (managedDeptId == null)
            {
                return Ok(new { IsSuccessed = false, Message = "Bạn không quản lý phòng ban nào" });
            }

            var result = await _userService.GetDepartmentUsersAsync(managedDeptId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin hồ sơ của mình
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _userService.GetProfileInfoAsync(UserId);
            return Ok(new { IsSuccessed = true, ResultObj = result });
        }
    }
}
