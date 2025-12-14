using BaseSource.Services.Services.Department;
using BaseSource.Services.Services.Salary;
using BaseSource.ViewModels.Salary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.Controllers
{
    [Route("api/salary")]
    [ApiController]
    [Authorize]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;
        private readonly IDepartmentService _departmentService;

        public SalaryController(ISalaryService salaryService, IDepartmentService departmentService)
        {
            _salaryService = salaryService;
            _departmentService = departmentService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Tính lương (Admin)
        /// </summary>
        [HttpPost("calculate")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CalculateSalary([FromBody] CalculateSalaryRequest request)
        {
            var result = await _salaryService.CalculateSalaryAsync(UserId, request);
            return Ok(result);
        }

        /// <summary>
        /// Xem trước bảng tính lương (Admin)
        /// </summary>
        [HttpPost("preview-calculate")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CalculateSalaryPreview([FromBody] CalculateSalaryRequest request)
        {
            var result = await _salaryService.CalculateSalaryPreviewAsync(UserId, request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy phiếu lương của mình
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMySalary([FromQuery] int? month, [FromQuery] int? year)
        {
            var result = await _salaryService.GetMySalaryAsync(UserId, month, year);
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả phiếu lương (Admin only) - với filter phòng ban và paging
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetAllSalary(
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] Guid? departmentId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _salaryService.GetAllSalaryAsync(month, year, departmentId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy phiếu lương theo phòng ban (Manager/Deputy only - chỉ phòng ban mình quản lý)
        /// </summary>
        [HttpGet("department")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,DeputyManager")]
        public async Task<IActionResult> GetDepartmentSalary([FromQuery] int month, [FromQuery] int year)
        {
            // Get the department this user manages
            var managedDeptId = await _departmentService.GetManagedDepartmentIdAsync(UserId);

            if (managedDeptId == null)
            {
                return Ok(new { IsSuccessed = false, Message = "Bạn không quản lý phòng ban nào" });
            }

            var result = await _salaryService.GetDepartmentSalaryAsync(managedDeptId.Value, month, year);
            return Ok(result);
        }

        /// <summary>
        /// Duyệt phiếu lương (Admin/Manager)
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,DeputyManager")]
        public async Task<IActionResult> ApproveSalary(string id)
        {
            var result = await _salaryService.ApproveSalaryAsync(UserId, id);
            return Ok(result);
        }

        /// <summary>
        /// Phát lương (Admin/Manager)
        /// </summary>
        [HttpPost("{id}/pay")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,DeputyManager")]
        public async Task<IActionResult> PaySalary(string id, [FromBody] PaySalaryRequest? request)
        {
            var result = await _salaryService.PaySalaryAsync(UserId, id, request?.Note);
            return Ok(result);
        }

        /// <summary>
        /// Xác nhận lương (Employee)
        /// </summary>
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmSalary(string id, [FromBody] ConfirmSalaryRequest request)
        {
            var result = await _salaryService.ConfirmSalaryAsync(UserId, id, request.IsConfirmed, request.Note);
            return Ok(result);
        }

        /// <summary>
        /// Xem lịch sử phát lương
        /// </summary>
        [HttpGet("history")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager,DeputyManager")]
        public async Task<IActionResult> GetPaymentHistory([FromQuery] SalaryHistoryQuery query)
        {
            var result = await _salaryService.GetPaymentHistoryAsync(query);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật thông tin lương (Admin/Manager)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin,Manager,DeputyManager")]
        public async Task<IActionResult> UpdateSalary(string id, [FromBody] UpdateSalaryRequest request)
        {
            var result = await _salaryService.UpdateSalaryAsync(UserId, id, request);
            return Ok(result);
        }

        // ==================== COMPLAINTS ====================

        /// <summary>
        /// Tạo khiếu nại lương (Employee)
        /// </summary>
        [HttpPost("complaints")]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequest request)
        {
            var result = await _salaryService.CreateComplaintAsync(UserId, request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy khiếu nại của mình (Employee)
        /// </summary>
        [HttpGet("complaints/my")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var result = await _salaryService.GetMyComplaintsAsync(UserId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả khiếu nại (Admin only)
        /// </summary>
        [HttpGet("complaints")]
        [Authorize(Roles = "SuperAdmin,Admin,HR")]
        public async Task<IActionResult> GetAllComplaints()
        {
            var result = await _salaryService.GetAllComplaintsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Xử lý khiếu nại (Admin only)
        /// </summary>
        [HttpPost("complaints/{id}/resolve")]
        [Authorize(Roles = "SuperAdmin,Admin,HR")]
        public async Task<IActionResult> ResolveComplaint(Guid id, [FromBody] ResolveComplaintRequest request)
        {
            var result = await _salaryService.ResolveComplaintAsync(UserId, id, request);
            return Ok(result);
        }
    }
}
