using BaseSouce.Services.Services.User;
using BaseSource.Data.Entities;
using BaseSource.Services.Services.LeaveRequest;
using BaseSource.Services.Services.OvertimeRequest;
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

            return services;
        }
    }
}
