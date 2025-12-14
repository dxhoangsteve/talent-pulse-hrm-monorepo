
using BaseSource.Shared.Results;
using System.Threading.Tasks;

namespace BaseSource.Services.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<ApiResult<AdminDashboardStats>> GetAdminDashboardStatsAsync();
    }

    public class AdminDashboardStats
    {
        public int TotalUsers { get; set; }
        public int TotalDepartments { get; set; }
        public int PendingLeaveRequests { get; set; }
        public decimal TotalSalaryPaidThisMonth { get; set; }
    }
}
