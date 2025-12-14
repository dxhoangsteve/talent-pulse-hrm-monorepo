using BaseSouce.Services.Services.User;
using BaseSource.Data.Entities;
using BaseSource.Services.Services.Attendance;
using BaseSource.Services.Services.LeaveRequest;
using BaseSource.Services.Services.OvertimeRequest;
using BaseSource.Services.Services.Salary;
using Microsoft.AspNetCore.Identity;

namespace BaseSource.API.Cofigurations
{
    public static class DIConfigurations
    {
        public static IServiceCollection DIConfiguration(this IServiceCollection services)
        {
            // Identity Managers
            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();

            // Application Services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<BaseSource.Services.Services.Department.IDepartmentService, BaseSource.Services.Services.Department.DepartmentService>();
            services.AddTransient<ILeaveRequestService, LeaveRequestService>();
            services.AddTransient<IOvertimeRequestService, OvertimeRequestService>();
            services.AddTransient<IAttendanceService, AttendanceService>();
            services.AddTransient<ISalaryService, SalaryService>();
            services.AddTransient<BaseSource.Services.Services.Dashboard.IDashboardService, BaseSource.Services.Services.Dashboard.DashboardService>();

            return services;
        }
    }
}

