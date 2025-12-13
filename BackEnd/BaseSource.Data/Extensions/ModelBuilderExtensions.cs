using BaseSource.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaseSource.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            SeedRoles(modelBuilder);
            SeedAdminUser(modelBuilder);
            SeedDepartments(modelBuilder);
            SeedHolidays(modelBuilder);
        }

        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new[]
            {
                new AppRole { Id = "role-superadmin", Name = "SuperAdmin", NormalizedName = "SUPERADMIN", Description = "Super Administrator - Full access" },
                new AppRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN", Description = "Administrator - System management" },
                new AppRole { Id = "role-hr", Name = "HR", NormalizedName = "HR", Description = "Human Resources - HR management" },
                new AppRole { Id = "role-manager", Name = "Manager", NormalizedName = "MANAGER", Description = "Manager - Department management" },
                new AppRole { Id = "role-employee", Name = "Employee", NormalizedName = "EMPLOYEE", Description = "Employee - Basic access" }
            };

            modelBuilder.Entity<AppRole>().HasData(roles);
        }

        private static void SeedAdminUser(ModelBuilder modelBuilder)
        {
            var hasher = new PasswordHasher<AppUser>();
            var adminId = "user-superadmin";
            var adminEmployeeId = "employee-superadmin";
            var staticPasswordHash = hasher.HashPassword(null, "12345678");

            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = adminId,
                UserName = "superadmin",
                NormalizedUserName = "SUPERADMIN",
                Email = "admin@talentpulse.com",
                NormalizedEmail = "ADMIN@TALENTPULSE.COM",
                EmailConfirmed = true,
                PasswordHash = staticPasswordHash,
                SecurityStamp = "d7b00350-1372-4d4b-97e2-47525287515a",
                ConcurrencyStamp = "c8b00350-1372-4d4b-97e2-47525287515a",
                FullName = "Super Admin",
                IsActive = true,
                CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            modelBuilder.Entity<AppUserRole>().HasData(new AppUserRole
            {
                UserId = adminId,
                RoleId = "role-superadmin"
            });

            // Create Employee for superadmin so attendance/leave/salary features work
            modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = adminEmployeeId,
                EmployeeCode = "ADMIN001",
                FullName = "Super Admin",
                Email = "admin@talentpulse.com",
                UserId = adminId,
                BaseSalary = 50000000, // 50M VND
                JoinDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }

        private static void SeedDepartments(ModelBuilder modelBuilder)
        {
            var departments = new[]
            {
                new Department { Id = new Guid("26dd2648-8b9a-4c28-9a99-92c19f18e916"), Name = "Information Technology", Description = "Phòng Công nghệ Thông tin", CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { Id = new Guid("a7b00350-1372-4d4b-97e2-47525287515a"), Name = "Human Resources", Description = "Phòng Nhân sự", CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { Id = new Guid("5166c40a-d833-4f93-b0e6-999320623a7e"), Name = "Finance", Description = "Phòng Tài chính - Kế toán", CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { Id = new Guid("6c4d7f11-9e8c-4860-84a1-096950275816"), Name = "Marketing", Description = "Phòng Marketing", CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Department { Id = new Guid("df0ec670-348f-4952-bcc6-35327ec29505"), Name = "Operations", Description = "Phòng Vận hành", CreatedTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            };

            modelBuilder.Entity<Department>().HasData(departments);
        }

        private static void SeedHolidays(ModelBuilder modelBuilder)
        {
            var holidays = new[]
            {
                // 2024 Holidays
                new Holiday { Id = "hol-2024-newyear", Name = "Tết Dương lịch", Date = new DateTime(2024, 1, 1), Year = 2024, IsRecurring = true },
                new Holiday { Id = "hol-2024-tet1", Name = "Tết Nguyên đán", Date = new DateTime(2024, 2, 8), Year = 2024, IsRecurring = false },
                new Holiday { Id = "hol-2024-tet2", Name = "Tết Nguyên đán", Date = new DateTime(2024, 2, 9), Year = 2024, IsRecurring = false },
                new Holiday { Id = "hol-2024-tet3", Name = "Tết Nguyên đán", Date = new DateTime(2024, 2, 10), Year = 2024, IsRecurring = false },
                new Holiday { Id = "hol-2024-tet4", Name = "Tết Nguyên đán", Date = new DateTime(2024, 2, 11), Year = 2024, IsRecurring = false },
                new Holiday { Id = "hol-2024-tet5", Name = "Tết Nguyên đán", Date = new DateTime(2024, 2, 12), Year = 2024, IsRecurring = false },
                new Holiday { Id = "hol-2024-hung", Name = "Giỗ Tổ Hùng Vương", Date = new DateTime(2024, 4, 18), Year = 2024, IsRecurring = false },
                new Holiday { Id = "hol-2024-304", Name = "Ngày Giải phóng miền Nam", Date = new DateTime(2024, 4, 30), Year = 2024, IsRecurring = true },
                new Holiday { Id = "hol-2024-015", Name = "Ngày Quốc tế Lao động", Date = new DateTime(2024, 5, 1), Year = 2024, IsRecurring = true },
                new Holiday { Id = "hol-2024-029", Name = "Ngày Quốc khánh", Date = new DateTime(2024, 9, 2), Year = 2024, IsRecurring = true },
                
                // 2025 Holidays
                new Holiday { Id = "hol-2025-newyear", Name = "Tết Dương lịch", Date = new DateTime(2025, 1, 1), Year = 2025, IsRecurring = true },
                new Holiday { Id = "hol-2025-tet1", Name = "Tết Nguyên đán", Date = new DateTime(2025, 1, 28), Year = 2025, IsRecurring = false },
                new Holiday { Id = "hol-2025-tet2", Name = "Tết Nguyên đán", Date = new DateTime(2025, 1, 29), Year = 2025, IsRecurring = false },
                new Holiday { Id = "hol-2025-tet3", Name = "Tết Nguyên đán", Date = new DateTime(2025, 1, 30), Year = 2025, IsRecurring = false },
                new Holiday { Id = "hol-2025-tet4", Name = "Tết Nguyên đán", Date = new DateTime(2025, 1, 31), Year = 2025, IsRecurring = false },
                new Holiday { Id = "hol-2025-tet5", Name = "Tết Nguyên đán", Date = new DateTime(2025, 2, 1), Year = 2025, IsRecurring = false },
                new Holiday { Id = "hol-2025-hung", Name = "Giỗ Tổ Hùng Vương", Date = new DateTime(2025, 4, 7), Year = 2025, IsRecurring = false },
                new Holiday { Id = "hol-2025-304", Name = "Ngày Giải phóng miền Nam", Date = new DateTime(2025, 4, 30), Year = 2025, IsRecurring = true },
                new Holiday { Id = "hol-2025-015", Name = "Ngày Quốc tế Lao động", Date = new DateTime(2025, 5, 1), Year = 2025, IsRecurring = true },
                new Holiday { Id = "hol-2025-029", Name = "Ngày Quốc khánh", Date = new DateTime(2025, 9, 2), Year = 2025, IsRecurring = true }
            };

            modelBuilder.Entity<Holiday>().HasData(holidays);
        }
    }
}
