
using BaseSource.Data.EF;
using BaseSource.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.Services.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly BaseSourceDbContext _context;

        public DashboardService(BaseSourceDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<AdminDashboardStats>> GetAdminDashboardStatsAsync()
        {
            var today = DateTime.Now;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            try
            {
                var totalUsers = await _context.Users.CountAsync(u => !u.IsDeleted);
                var totalDepartments = await _context.Departments.CountAsync();

                // Pending Leave Requests (Status = 0)
                var pendingLeaves = await _context.LeaveRequests.CountAsync(l => l.Status == 0);

                // Total Salary Paid This Month (Status = 3)
                var totalSalary = await _context.Salaries
                    .Where(s => s.Month == currentMonth && s.Year == currentYear && s.Status == 3) // Paid
                    .SumAsync(s => s.NetSalary);

                var stats = new AdminDashboardStats
                {
                    TotalUsers = totalUsers,
                    TotalDepartments = totalDepartments,
                    PendingLeaveRequests = pendingLeaves,
                    TotalSalaryPaidThisMonth = totalSalary
                };

                return new ApiResult<AdminDashboardStats> { IsSuccessed = true, ResultObj = stats };
            }
            catch (Exception ex)
            {
                return new ApiResult<AdminDashboardStats> { IsSuccessed = false, Message = ex.Message };
            }
        }
    }
}
