using BaseSource.Data.Entities;
using BaseSource.Data.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaseSource.Data.EF
{
    public class BaseSourceDbContext : IdentityDbContext<AppUser, AppRole, string, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
    {
        public BaseSourceDbContext(DbContextOptions options) : base(options)
        {
        }

        // Core
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        
        // HR Management
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<OvertimeRequest> OvertimeRequests { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        
        // Payroll
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<SalaryComplaint> SalaryComplaints { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            #region Identity Tables
            builder.Entity<AppUserClaim>().ToTable("AppUserClaims");
            builder.Entity<AppUserLogin>().ToTable("AppUserLogins");
            builder.Entity<AppUserToken>().ToTable("AppUserTokens");
            builder.Entity<AppRoleClaim>().ToTable("AppRoleClaims");
            
            // Configure AppUser BaseSalary
            builder.Entity<AppUser>().Property(x => x.BaseSalary).HasPrecision(18, 2);
            #endregion

            // Apply all configurations
            builder.ApplyConfigurationsFromAssembly(typeof(BaseSourceDbContext).Assembly);
            
            // Data seeding
            builder.Seed();
        }
    }
}
