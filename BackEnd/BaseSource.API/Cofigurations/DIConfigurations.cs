using BaseSouce.Services.Services.User;
using BaseSouce.Services.Services.UserAdmin;
using BaseSource.Data.Entities;
using BaseSource.Services.Services.Setting;
using Microsoft.AspNetCore.Identity;

namespace BaseSource.API.Cofigurations
{
    public static class DIConfigurations
    {
        public static IServiceCollection DIConfiguration(this IServiceCollection services)
        {
            //DI for user
            services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
            services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserAdminService, UserAdminService>();

            services.AddTransient<IConfigSettingService, ConfigSettingService>();
            return services;
        }
    }
}
