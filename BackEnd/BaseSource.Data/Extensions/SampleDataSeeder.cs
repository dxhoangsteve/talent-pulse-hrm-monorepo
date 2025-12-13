using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaseSource.Data.Extensions
{
    public static class SampleDataSeeder
    {
        public static async Task SeedSampleDataAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BaseSourceDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            
            // Check if already seeded
            if (await context.Employees.CountAsync() > 5)
            {
                Console.WriteLine("📊 Sample data already exists, skipping...");
                return;
            }

            Console.WriteLine("📊 Seeding sample data...");
            
            var hasher = new PasswordHasher<AppUser>();
            var baseDate = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Update departments to Vietnamese names
            var itDeptId = new Guid("26dd2648-8b9a-4c28-9a99-92c19f18e916");
            var hrDeptId = new Guid("a7b00350-1372-4d4b-97e2-47525287515a");
            var financeDeptId = new Guid("5166c40a-d833-4f93-b0e6-999320623a7e");
            var marketingDeptId = new Guid("6c4d7f11-9e8c-4860-84a1-096950275816");
            var opsDeptId = new Guid("df0ec670-348f-4952-bcc6-35327ec29505");

            // Update department names to Vietnamese
            var depts = await context.Departments.ToListAsync();
            foreach (var d in depts)
            {
                if (d.Id == itDeptId) { d.Name = "Phòng Công nghệ Thông tin"; }
                else if (d.Id == hrDeptId) { d.Name = "Phòng Nhân sự"; }
                else if (d.Id == financeDeptId) { d.Name = "Phòng Tài chính - Kế toán"; }
                else if (d.Id == marketingDeptId) { d.Name = "Phòng Marketing"; }
                else if (d.Id == opsDeptId) { d.Name = "Phòng Vận hành"; }
            }
            await context.SaveChangesAsync();

            // Create sample employees with users
            var employeeData = new List<(string id, string name, string email, string username, Guid deptId, decimal salary, string role, PositionType position)>
            {
                // IT Department - Trưởng phòng + nhân viên
                ("emp-it-manager", "Nguyễn Văn An", "an.nguyen@company.com", "an.nguyen", itDeptId, 35000000, "Manager", PositionType.Manager),
                ("emp-it-deputy", "Trần Thị Bình", "binh.tran@company.com", "binh.tran", itDeptId, 28000000, "Manager", PositionType.DeputyManager),
                ("emp-it-01", "Lê Hoàng Cường", "cuong.le@company.com", "cuong.le", itDeptId, 20000000, "Employee", PositionType.Senior),
                ("emp-it-02", "Phạm Minh Đức", "duc.pham@company.com", "duc.pham", itDeptId, 18000000, "Employee", PositionType.Junior),
                ("emp-it-03", "Hoàng Thu Hà", "ha.hoang@company.com", "ha.hoang", itDeptId, 22000000, "Employee", PositionType.Senior),
                
                // HR Department
                ("emp-hr-manager", "Võ Thanh Hùng", "hung.vo@company.com", "hung.vo", hrDeptId, 32000000, "Manager", PositionType.Manager),
                ("emp-hr-01", "Ngô Thị Kim", "kim.ngo@company.com", "kim.ngo", hrDeptId, 17000000, "Employee", PositionType.Junior),
                
                // Finance Department
                ("emp-fin-manager", "Đỗ Văn Long", "long.do@company.com", "long.do", financeDeptId, 38000000, "Manager", PositionType.Manager),
                ("emp-fin-01", "Bùi Thị Mai", "mai.bui@company.com", "mai.bui", financeDeptId, 19000000, "Employee", PositionType.Junior),
                ("emp-fin-02", "Dương Hoàng Nam", "nam.duong@company.com", "nam.duong", financeDeptId, 21000000, "Employee", PositionType.Senior),
                
                // Marketing
                ("emp-mkt-manager", "Lý Thị Oanh", "oanh.ly@company.com", "oanh.ly", marketingDeptId, 30000000, "Manager", PositionType.Manager),
                ("emp-mkt-01", "Trương Văn Phúc", "phuc.truong@company.com", "phuc.truong", marketingDeptId, 16000000, "Employee", PositionType.Junior),
                
                // Operations
                ("emp-ops-manager", "Đinh Minh Quân", "quan.dinh@company.com", "quan.dinh", opsDeptId, 29000000, "Manager", PositionType.Manager),
                ("emp-ops-01", "Huỳnh Thị Như", "nhu.huynh@company.com", "nhu.huynh", opsDeptId, 15000000, "Employee", PositionType.Junior),
            };

            var createdEmployees = new List<Employee>();
            
            foreach (var (id, name, email, username, deptId, salary, role, position) in employeeData)
            {
                // Create user
                var userId = $"user-{id}";
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
                    FullName = name,
                    IsActive = true,
                    Position = position,
                    DepartmentId = deptId,
                    BaseSalary = salary,
                    CreatedTime = baseDate
                };
                context.Users.Add(user);
                
                // Add role
                var userRole = new AppUserRole { UserId = userId, RoleId = role == "Manager" ? "role-manager" : "role-employee" };
                context.UserRoles.Add(userRole);
                
                // Create employee
                var employee = new Employee
                {
                    Id = id,
                    EmployeeCode = $"NV{id.GetHashCode():X8}".Substring(0, 10),
                    FullName = name,
                    Email = email,
                    UserId = userId,
                    DepartmentId = deptId,
                    BaseSalary = salary,
                    JoinDate = baseDate.AddMonths(-new Random(id.GetHashCode()).Next(6, 24)),
                    CreatedTime = baseDate
                };
                context.Employees.Add(employee);
                createdEmployees.Add(employee);
            }
            await context.SaveChangesAsync();

            // Update department managers
            var itDept = await context.Departments.FindAsync(itDeptId);
            if (itDept != null) { itDept.ManagerId = "user-emp-it-manager"; itDept.DeputyId = "user-emp-it-deputy"; }
            
            var hrDept = await context.Departments.FindAsync(hrDeptId);
            if (hrDept != null) { hrDept.ManagerId = "user-emp-hr-manager"; }
            
            var finDept = await context.Departments.FindAsync(financeDeptId);
            if (finDept != null) { finDept.ManagerId = "user-emp-fin-manager"; }
            
            var mktDept = await context.Departments.FindAsync(marketingDeptId);
            if (mktDept != null) { mktDept.ManagerId = "user-emp-mkt-manager"; }
            
            var opsDept = await context.Departments.FindAsync(opsDeptId);
            if (opsDept != null) { opsDept.ManagerId = "user-emp-ops-manager"; }
            
            await context.SaveChangesAsync();

            // Seed attendance history (3 months: Oct, Nov, Dec 2024)
            var random = new Random(42);
            foreach (var emp in createdEmployees)
            {
                for (int month = 10; month <= 12; month++)
                {
                    var daysInMonth = DateTime.DaysInMonth(2024, month);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var date = new DateTime(2024, month, day, 0, 0, 0, DateTimeKind.Utc);
                        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                            continue;

                        var isLate = random.NextDouble() < 0.1; // 10% chance late
                        var checkInHour = isLate ? 8 + random.Next(1, 3) : 8;
                        var checkInMinute = random.Next(0, 30);
                        var checkOutHour = 17 + random.Next(0, 2);
                        var checkOutMinute = random.Next(0, 45);

                        var attendance = new Attendance
                        {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeId = emp.Id,
                            Date = date,
                            CheckInTime = new TimeSpan(checkInHour, checkInMinute, 0),
                            CheckOutTime = new TimeSpan(checkOutHour, checkOutMinute, 0),
                            Status = isLate ? AttendanceStatus.Late : AttendanceStatus.Present,
                            WorkHours = (decimal)(checkOutHour - checkInHour) + (checkOutMinute - checkInMinute) / 60m,
                            CreatedTime = date
                        };
                        context.Attendances.Add(attendance);
                    }
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Created {await context.Attendances.CountAsync()} attendance records");

            // Seed leave requests
            var leaveTypes = new[] { LeaveType.Annual, LeaveType.Sick, LeaveType.Unpaid };
            var statuses = new[] { RequestStatus.Pending, RequestStatus.Approved, RequestStatus.Rejected };
            
            foreach (var emp in createdEmployees.Take(10))
            {
                var numRequests = random.Next(1, 4);
                for (int i = 0; i < numRequests; i++)
                {
                    var month = random.Next(10, 13);
                    var day = random.Next(1, 25);
                    var startDate = new DateTime(2024, month, day, 0, 0, 0, DateTimeKind.Utc);
                    var endDate = startDate.AddDays(random.Next(1, 4));
                    var status = statuses[random.Next(statuses.Length)];
                    
                    var leave = new LeaveRequest
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = emp.Id,
                        LeaveType = leaveTypes[random.Next(leaveTypes.Length)],
                        StartDate = startDate,
                        EndDate = endDate,
                        TotalDays = (decimal)(endDate - startDate).TotalDays + 1,
                        Reason = $"Xin nghỉ phép {i + 1}",
                        Status = status,
                        ApprovedBy = status != RequestStatus.Pending ? "user-superadmin" : null,
                        ApprovedTime = status != RequestStatus.Pending ? startDate.AddDays(-1) : null,
                        CreatedTime = startDate.AddDays(-5)
                    };
                    context.LeaveRequests.Add(leave);
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Created {await context.LeaveRequests.CountAsync()} leave requests");

            // Seed OT requests
            foreach (var emp in createdEmployees.Take(8))
            {
                var numRequests = random.Next(2, 5);
                for (int i = 0; i < numRequests; i++)
                {
                    var month = random.Next(10, 13);
                    var day = random.Next(1, 28);
                    var otDate = new DateTime(2024, month, day, 0, 0, 0, DateTimeKind.Utc);
                    if (otDate.DayOfWeek == DayOfWeek.Sunday) continue;
                    
                    var status = statuses[random.Next(statuses.Length)];
                    var startHour = 18;
                    var endHour = 18 + random.Next(1, 4);
                    var multiplier = otDate.DayOfWeek == DayOfWeek.Saturday ? 2.0m : 1.5m;
                    
                    var ot = new OvertimeRequest
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = emp.Id,
                        OTDate = otDate,
                        StartTime = new TimeSpan(startHour, 0, 0),
                        EndTime = new TimeSpan(endHour, 0, 0),
                        Hours = endHour - startHour,
                        Multiplier = multiplier,
                        Reason = $"Làm thêm dự án {random.Next(1, 5)}",
                        Status = status,
                        ApprovedBy = status != RequestStatus.Pending ? "user-superadmin" : null,
                        ApprovedTime = status != RequestStatus.Pending ? otDate.AddDays(-1) : null,
                        CreatedTime = otDate.AddDays(-3)
                    };
                    context.OvertimeRequests.Add(ot);
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Created {await context.OvertimeRequests.CountAsync()} OT requests");

            // Seed salary data (3 months)
            var salaryStatuses = new[] { SalaryStatus.Paid, SalaryStatus.Paid, SalaryStatus.Draft };
            foreach (var emp in createdEmployees)
            {
                for (int monthOffset = 0; monthOffset < 3; monthOffset++)
                {
                    var month = 10 + monthOffset;
                    var status = monthOffset == 2 ? SalaryStatus.Draft : SalaryStatus.Paid;
                    
                    var baseSalary = emp.BaseSalary;
                    var bonus = random.Next(0, 5) * 500000m;
                    var otPay = random.Next(0, 10) * 200000m;
                    var deductions = random.Next(0, 3) * 100000m;
                    var insurance = baseSalary * 0.105m;
                    var tax = baseSalary > 15000000 ? (baseSalary - 11000000) * 0.1m : 0;
                    
                    var salary = new Salary
                    {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = emp.Id,
                        Month = month,
                        Year = 2024,
                        WorkDays = 22,
                        ActualWorkDays = 22 - random.Next(0, 3),
                        BaseSalary = baseSalary,
                        OvertimePay = otPay,
                        Bonus = bonus,
                        Allowance = 2000000,
                        Deductions = deductions,
                        Insurance = insurance,
                        Tax = tax,
                        NetSalary = baseSalary + otPay + bonus + 2000000 - deductions - insurance - tax,
                        Status = status,
                        PaidTime = status == SalaryStatus.Paid ? new DateTime(2024, month, 28, 0, 0, 0, DateTimeKind.Utc) : null,
                        PaidBy = status == SalaryStatus.Paid ? "user-superadmin" : null,
                        CreatedTime = new DateTime(2024, month, 25, 0, 0, 0, DateTimeKind.Utc)
                    };
                    context.Salaries.Add(salary);
                }
            }
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Created {await context.Salaries.CountAsync()} salary records");
            
            Console.WriteLine("📊 Sample data seeding completed!");
        }
    }
}
