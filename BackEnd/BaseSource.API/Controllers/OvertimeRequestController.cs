using BaseSource.Services.Services.OvertimeRequest;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.OvertimeRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.Controllers
{
    [ApiController]
    [Route("api/overtime-requests")]
    [Authorize]
    public class OvertimeRequestController : ControllerBase
    {
        private readonly IOvertimeRequestService _overtimeRequestService;

        public OvertimeRequestController(IOvertimeRequestService overtimeRequestService)
        {
            _overtimeRequestService = overtimeRequestService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        /// <summary>
        /// Tạo đơn xin OT
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOvertimeRequestVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _overtimeRequestService.CreateAsync(GetUserId(), model);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách đơn OT của bản thân
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRequests()
        {
            var result = await _overtimeRequestService.GetMyRequestsAsync(GetUserId());
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết đơn OT
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _overtimeRequestService.GetByIdAsync(id);
            if (!result.IsSuccessed)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả đơn OT - Admin (với filter phòng ban, status, paging)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "SuperAdmin,Admin,HR")]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? departmentId = null,
            [FromQuery] RequestStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _overtimeRequestService.GetAllAsync(departmentId, status, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách đơn OT chờ duyệt (cho Admin/Manager/Deputy)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager")]
        public async Task<IActionResult> GetPendingForApproval()
        {
            var result = await _overtimeRequestService.GetPendingForApprovalAsync(GetUserId());
            return Ok(result);
        }

        /// <summary>
        /// Duyệt đơn OT
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager")]
        public async Task<IActionResult> Approve(string id)
        {
            var result = await _overtimeRequestService.ApproveAsync(id, GetUserId());
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Từ chối đơn OT
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager")]
        public async Task<IActionResult> Reject(string id, [FromBody] RejectRequestVm model)
        {
            var result = await _overtimeRequestService.RejectAsync(id, GetUserId(), model.RejectReason);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Hủy đơn OT (chỉ người tạo có thể hủy)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            var result = await _overtimeRequestService.CancelAsync(id, GetUserId());
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
