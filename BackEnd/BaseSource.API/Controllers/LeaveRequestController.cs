using BaseSource.Services.Services.LeaveRequest;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.LeaveRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaseSource.API.Controllers
{
    [ApiController]
    [Route("api/leave-requests")]
    [Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        /// <summary>
        /// Tạo đơn xin nghỉ phép
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _leaveRequestService.CreateAsync(GetUserId(), model);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách đơn nghỉ phép của bản thân
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyRequests()
        {
            var result = await _leaveRequestService.GetMyRequestsAsync(GetUserId());
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết đơn nghỉ phép
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _leaveRequestService.GetByIdAsync(id);
            if (!result.IsSuccessed)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách đơn chờ duyệt (cho Admin/Manager/Deputy)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager")]
        public async Task<IActionResult> GetPendingForApproval()
        {
            var result = await _leaveRequestService.GetPendingForApprovalAsync(GetUserId());
            return Ok(result);
        }

        /// <summary>
        /// Duyệt đơn nghỉ phép
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager")]
        public async Task<IActionResult> Approve(string id)
        {
            var result = await _leaveRequestService.ApproveAsync(id, GetUserId());
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Từ chối đơn nghỉ phép
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "SuperAdmin,Admin,HR,Manager")]
        public async Task<IActionResult> Reject(string id, [FromBody] RejectRequestVm model)
        {
            var result = await _leaveRequestService.RejectAsync(id, GetUserId(), model.RejectReason);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Hủy đơn nghỉ phép (chỉ người tạo có thể hủy)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            var result = await _leaveRequestService.CancelAsync(id, GetUserId());
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
