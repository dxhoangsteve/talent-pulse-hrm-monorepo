
using BaseSource.Services.Services.Dashboard;
using BaseSource.Shared.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BaseSource.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin-stats")]
        public async Task<IActionResult> GetAdminDashboardStats()
        {
            var result = await _dashboardService.GetAdminDashboardStatsAsync();
            return Ok(result);
        }
    }
}
