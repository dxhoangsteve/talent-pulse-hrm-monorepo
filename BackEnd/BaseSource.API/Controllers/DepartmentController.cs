using BaseSource.Services.Services.Department;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BaseSource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<bool>(ModelState.GetListErrors()));
            }
            var result = await _departmentService.CreateAsync(request);
            if (!result.IsSuccessed)
            {
                return Ok(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmentService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật trưởng/phó phòng (chỉ Admin)
        /// </summary>
        [HttpPut("{id}/leadership")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateLeadership(Guid id, [FromBody] UpdateDepartmentLeadershipVm model)
        {
            var result = await _departmentService.UpdateLeadershipAsync(id, model);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách user có thể làm trưởng/phó phòng
        /// </summary>
        [HttpGet("available-leaders")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> GetAvailableLeaders()
        {
            var result = await _departmentService.GetAvailableLeadersAsync();
            return Ok(result);
        }
    }
}
