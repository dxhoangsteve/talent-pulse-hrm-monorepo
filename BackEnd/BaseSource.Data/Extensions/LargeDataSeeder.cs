using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BaseSource.Shared.Helpers;

namespace BaseSource.Data.Extensions
{
    public static class LargeDataSeeder
    {
        public static async Task SeedLargeDataAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BaseSourceDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            // Check if already seeded large data
            if (await context.Employees.CountAsync() > 200)
            {
                Console.WriteLine("📊 Large sample data already exists (>200), skipping...");
                return;
            }

            Console.WriteLine("📊 Seeding LARGE sample data (target ~250 employees)...");

            var hasher = new PasswordHasher<AppUser>();
            var baseDate = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Unspecified);

            // Get Departments
            var depts = await context.Departments.ToListAsync();
            if (!depts.Any()) return;

            // Generators
            string[] firstNames = { "Tuấn", "Dũng", "Hồng", "Mai", "Lan", "Hương", "Hà", "Minh", "Đức", "Thành", "Vinh", "Khoa", "Ngọc", "Thảo", "Trang", "Linh", "Kiên", "Cường", "Hải", "Sơn", "Tâm", "Thắng", "Quang", "Đạt", "Tú", "Nhung", "Yến", "Phương" };
            string[] lastNames = { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương", "Lý", "Đinh", "Đoàn", "Lâm", "Trịnh" };

            var randomGen = new Random(456);
            var employeesToCreate = new List<Employee>();
            var usersToCreate = new List<AppUser>();
            var userRolesToCreate = new List<AppUserRole>();

            int currentCount = await context.Employees.CountAsync();
            int targetCount = 250;
            int count = currentCount + 1;

            while (count <= targetCount)
            {
                var dept = depts[randomGen.Next(depts.Count)];
                var firstName = firstNames[randomGen.Next(firstNames.Length)];
                var lastName = lastNames[randomGen.Next(lastNames.Length)];
                var fullName = $"{lastName} {firstName}";
                var username = $"user{count}";
                var email = $"user{count}@company.com";

                // Varied salary based on role (simulate)
                var positionType = (PositionType)randomGen.Next(1, 6); // Random position
                decimal baseSalary = 10000000;

                switch (positionType)
                {
                    case PositionType.Junior: baseSalary = randomGen.Next(8, 15) * 1000000; break;
                    case PositionType.Senior: baseSalary = randomGen.Next(18, 30) * 1000000; break;
                    case PositionType.Manager: baseSalary = randomGen.Next(30, 50) * 1000000; break;
                    case PositionType.DeputyManager: baseSalary = randomGen.Next(25, 40) * 1000000; break;
                    case PositionType.Intern: baseSalary = randomGen.Next(2, 6) * 1000000; break;
                    default: baseSalary = randomGen.Next(10, 20) * 1000000; break;
                }

                var userId = $"user-gen-{count}";
                var empId = $"emp-gen-{count}";

                var user = new AppUser
                {
                    Id = userId,
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null!, "12345678"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    FullName = fullName,
                    IsActive = true,
                    Position = positionType,
                    DepartmentId = dept.Id,
                    BaseSalary = baseSalary,
                    CreatedTime = baseDate
                };
                usersToCreate.Add(user);

                var userRole = new AppUserRole { UserId = userId, RoleId = "role-employee" };
                userRolesToCreate.Add(userRole);

                var employee = new Employee
                {
                    Id = empId,
                    EmployeeCode = $"NV{count:0000}",
                    FullName = fullName,
                    Email = email,
                    UserId = userId,
                    DepartmentId = dept.Id,
                    BaseSalary = baseSalary,
                    Position = positionType.ToString(),
                    Level = (EmployeeLevel)randomGen.Next(0, 5),
                    JoinDate = baseDate.AddMonths(-randomGen.Next(1, 36)),
                    CreatedTime = baseDate,
                    Status = EmployeeStatus.Active
                };
                employeesToCreate.Add(employee);

                count++;
            }

            // Batch save
            await context.Users.AddRangeAsync(usersToCreate);
            await context.UserRoles.AddRangeAsync(userRolesToCreate);
            await context.Employees.AddRangeAsync(employeesToCreate);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Created {employeesToCreate.Count} additional employees.");

            // Generate related data for these new employees
            await GenerateAttendance(context, employeesToCreate, randomGen);
            await GenerateSalaries(context, employeesToCreate, randomGen);
            await GenerateLeaveAndOT(context, employeesToCreate, randomGen);

            Console.WriteLine("✅ Large Data Seeding Finished.");
        }

        private static async Task GenerateAttendance(BaseSourceDbContext context, List<Employee> employees, Random random)
        {
            var attendances = new List<Attendance>();
            var months = new[] { 10, 11, 12 };

            foreach (var emp in employees)
            {
                foreach (var month in months)
                {
                    var days = DateTime.DaysInMonth(2024, month);
                    for (int day = 1; day <= days; day++)
                    {
                        var date = new DateTime(2024, month, day);
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;

                        if (random.NextDouble() > 0.95) continue; // 5% absent

                        bool isLate = random.NextDouble() < 0.15; // 15% late
                        var checkInH = isLate ? 8 + random.Next(1, 2) : 8;
                        var checkInM = isLate ? random.Next(10, 59) : random.Next(0, 30);
                        var checkOutH = 17 + random.Next(0, 3);
                        var checkOutM = random.Next(0, 59);

                        var att = new Attendance
                        {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeId = emp.Id,
                            Date = date,
                            CheckInTime = new TimeSpan(checkInH, checkInM, 0),
                            CheckOutTime = new TimeSpan(checkOutH, checkOutM, 0),
                            Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present,
                            WorkHours = (decimal)(checkOutH - checkInH + (checkOutM - checkInM) / 60.0),
                            CreatedTime = date
                        };
                        attendances.Add(att);
                    }
                }
            }
            await context.Attendances.AddRangeAsync(attendances);
            await context.SaveChangesAsync();
            Console.WriteLine("    - Attendance data generated.");
        }

        private static async Task GenerateSalaries(BaseSourceDbContext context, List<Employee> employees, Random random)
        {
            var salaries = new List<Salary>();
            // Oct, Nov Paid. Dec Draft.

            foreach (var emp in employees)
            {
                // Oct & Nov
                for (int m = 10; m <= 11; m++)
                {
                    decimal bonus = random.Next(0, 3) * 500000;
                    decimal ot = random.Next(0, 5) * 200000;
                    decimal insurance = emp.BaseSalary * 0.105m;
                    decimal net = emp.BaseSalary + bonus + ot - insurance;

                    salaries.Add(new Salary
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = emp.Id,
                        Month = m,
                        Year = 2024,
                        BaseSalary = emp.BaseSalary,
                        WorkDays = 22,
                        ActualWorkDays = 22,
                        Bonus = bonus,
                        OvertimePay = ot,
                        Insurance = insurance,
                        NetSalary = net,
                        Status = SalaryStatus.Paid,
                        PaidTime = new DateTime(2024, m, 28),
                        CreatedTime = new DateTime(2024, m, 25)
                    });
                }
            }
            await context.Salaries.AddRangeAsync(salaries);
            await context.SaveChangesAsync();
            Console.WriteLine("    - Salary data generated.");
        }

        private static async Task GenerateLeaveAndOT(BaseSourceDbContext context, List<Employee> employees, Random random)
        {
            // Simple logic
            var leaves = new List<LeaveRequest>();
            foreach (var emp in employees.Take(50)) // Only some have requests
            {
                leaves.Add(new LeaveRequest
                {
                    EmployeeId = emp.Id,
                    LeaveType = LeaveType.Annual,
                    StartDate = new DateTime(2024, 11, random.Next(1, 20)),
                    EndDate = new DateTime(2024, 11, random.Next(21, 25)),
                    TotalDays = 1,
                    Reason = "Generated Leave",
                    Status = RequestStatus.Approved,
                    CreatedTime = new DateTime(2024, 11, 1)
                });
            }
            await context.LeaveRequests.AddRangeAsync(leaves);
            await context.SaveChangesAsync();
            Console.WriteLine("    - Leave requests generated.");
        }
    }
}
