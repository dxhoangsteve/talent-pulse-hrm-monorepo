using BaseSource.Data.EF;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Salary;
using Microsoft.EntityFrameworkCore;

namespace BaseSource.Services.Services.Salary
{
    public class SalaryService : ISalaryService
    {
        private readonly BaseSourceDbContext _context;

        public SalaryService(BaseSourceDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<SalaryVm>> CalculateSalaryAsync(string adminId, CalculateSalaryRequest request)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == request.EmployeeId);

                if (employee == null)
                {
                    return new ApiResult<SalaryVm> { IsSuccessed = false, Message = "Không tìm thấy nhân viên" };
                }

                // Check if salary already exists
                var existingSalary = await _context.Salaries
                    .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId && s.Month == request.Month && s.Year == request.Year);

                // Calculate attendance stats
                var startDate = new DateTime(request.Year, request.Month, 1);
                var endDate = startDate.AddMonths(1);
                
                var attendances = await _context.Attendances
                    .Where(a => a.EmployeeId == request.EmployeeId && a.Date >= startDate && a.Date < endDate)
                    .ToListAsync();

                int workDays = GetWorkDaysInMonth(request.Year, request.Month);
                decimal actualWorkDays = attendances.Count(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue);
                int lateDays = attendances.Count(a => a.Status == AttendanceStatus.Late);
                int earlyLeaveDays = attendances.Count(a => a.Status == AttendanceStatus.EarlyLeave);
                decimal totalOTHours = attendances.Sum(a => a.OvertimeHours);

                // Calculate salary components
                decimal baseSalary = employee.BaseSalary;
                decimal dailyRate = baseSalary / workDays;
                decimal actualBasePay = dailyRate * actualWorkDays;
                
                decimal otRate = dailyRate / 8 * 1.5m; // 1.5x hourly rate for OT
                decimal overtimePay = otRate * totalOTHours;
                
                decimal bonus = request.Bonus ?? 0;
                decimal allowance = request.Allowance ?? 0;
                decimal deductions = request.Deductions ?? 0;
                
                // Late/early deduction (50k per incident)
                deductions += (lateDays + earlyLeaveDays) * 50000;
                
                decimal insurance = baseSalary * 0.105m; // 10.5% insurance
                decimal taxableIncome = actualBasePay + overtimePay + bonus + allowance - deductions - insurance;
                decimal tax = CalculateTax(taxableIncome);
                
                decimal netSalary = actualBasePay + overtimePay + bonus + allowance - deductions - insurance - tax;

                var salary = existingSalary ?? new Data.Entities.Salary
                {
                    EmployeeId = request.EmployeeId,
                    Month = request.Month,
                    Year = request.Year,
                };

                salary.WorkDays = workDays;
                salary.ActualWorkDays = actualWorkDays;
                salary.LateDays = lateDays;
                salary.EarlyLeaveDays = earlyLeaveDays;
                salary.BaseSalary = baseSalary;
                salary.OvertimePay = overtimePay;
                salary.Bonus = bonus;
                salary.Allowance = allowance;
                salary.Deductions = deductions;
                salary.Insurance = insurance;
                salary.Tax = tax;
                salary.NetSalary = Math.Max(0, netSalary);
                salary.Status = SalaryStatus.Pending;
                salary.Note = request.Note;
                salary.UpdatedTime = DateTime.UtcNow;

                if (existingSalary == null)
                {
                    _context.Salaries.Add(salary);
                }

                await _context.SaveChangesAsync();

                return new ApiResult<SalaryVm>
                {
                    IsSuccessed = true,
                    ResultObj = await MapToVmAsync(salary)
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<SalaryVm> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<SalaryVm>>> GetMySalaryAsync(string userId, int? month, int? year)
        {
            try
            {
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId);

                if (employee == null)
                {
                    return new ApiResult<List<SalaryVm>> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };
                }

                var query = _context.Salaries
                    .Include(s => s.ApprovedByUser)
                    .Include(s => s.PaidByUser)
                    .Where(s => s.EmployeeId == employee.Id);

                if (month.HasValue)
                    query = query.Where(s => s.Month == month.Value);
                if (year.HasValue)
                    query = query.Where(s => s.Year == year.Value);

                var salaries = await query
                    .OrderByDescending(s => s.Year)
                    .ThenByDescending(s => s.Month)
                    .ToListAsync();

                var result = new List<SalaryVm>();
                foreach (var s in salaries)
                {
                    result.Add(await MapToVmAsync(s));
                }

                return new ApiResult<List<SalaryVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<SalaryVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<SalaryVm>>> GetAllSalaryAsync(int month, int year)
        {
            try
            {
                var salaries = await _context.Salaries
                    .Include(s => s.Employee)
                        .ThenInclude(e => e.Department)
                    .Include(s => s.ApprovedByUser)
                    .Include(s => s.PaidByUser)
                    .Where(s => s.Month == month && s.Year == year)
                    .OrderBy(s => s.Employee.FullName)
                    .ToListAsync();

                var result = new List<SalaryVm>();
                foreach (var s in salaries)
                {
                    result.Add(await MapToVmAsync(s));
                }

                return new ApiResult<List<SalaryVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<SalaryVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<SalaryVm>>> GetDepartmentSalaryAsync(Guid departmentId, int month, int year)
        {
            try
            {
                var salaries = await _context.Salaries
                    .Include(s => s.Employee)
                        .ThenInclude(e => e.Department)
                    .Include(s => s.ApprovedByUser)
                    .Include(s => s.PaidByUser)
                    .Where(s => s.Employee.DepartmentId == departmentId && s.Month == month && s.Year == year)
                    .OrderBy(s => s.Employee.FullName)
                    .ToListAsync();

                var result = new List<SalaryVm>();
                foreach (var s in salaries)
                {
                    result.Add(await MapToVmAsync(s));
                }

                return new ApiResult<List<SalaryVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<SalaryVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<bool>> ApproveSalaryAsync(string adminId, string salaryId)
        {
            try
            {
                var salary = await _context.Salaries.FindAsync(salaryId);
                if (salary == null)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy phiếu lương" };
                }

                if (salary.Status != SalaryStatus.Pending)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Phiếu lương không ở trạng thái chờ duyệt" };
                }

                salary.Status = SalaryStatus.Approved;
                salary.ApprovedBy = adminId;
                salary.ApprovedTime = DateTime.UtcNow;
                salary.UpdatedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<bool>> PaySalaryAsync(string adminId, string salaryId, string? note)
        {
            try
            {
                var salary = await _context.Salaries.FindAsync(salaryId);
                if (salary == null)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy phiếu lương" };
                }

                if (salary.Status != SalaryStatus.Approved)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Phiếu lương chưa được duyệt" };
                }

                salary.Status = SalaryStatus.Paid;
                salary.PaidBy = adminId;
                salary.PaidTime = DateTime.UtcNow;
                salary.Note = string.IsNullOrEmpty(note) ? salary.Note : note;
                salary.UpdatedTime = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<SalaryVm>>> GetPaymentHistoryAsync(SalaryHistoryQuery query)
        {
            try
            {
                var q = _context.Salaries
                    .Include(s => s.Employee)
                        .ThenInclude(e => e.Department)
                    .Include(s => s.ApprovedByUser)
                    .Include(s => s.PaidByUser)
                    .Where(s => s.Status == SalaryStatus.Paid);

                if (query.Month.HasValue)
                    q = q.Where(s => s.Month == query.Month.Value);
                if (query.Year.HasValue)
                    q = q.Where(s => s.Year == query.Year.Value);
                if (!string.IsNullOrEmpty(query.EmployeeId))
                    q = q.Where(s => s.EmployeeId == query.EmployeeId);

                var salaries = await q
                    .OrderByDescending(s => s.PaidTime)
                    .ToListAsync();

                var result = new List<SalaryVm>();
                foreach (var s in salaries)
                {
                    result.Add(await MapToVmAsync(s));
                }

                return new ApiResult<List<SalaryVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<SalaryVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        private async Task<SalaryVm> MapToVmAsync(Data.Entities.Salary salary)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == salary.EmployeeId);

            return new SalaryVm
            {
                Id = salary.Id,
                EmployeeId = salary.EmployeeId,
                EmployeeName = employee?.FullName ?? "N/A",
                DepartmentName = employee?.Department?.Name,
                Month = salary.Month,
                Year = salary.Year,
                WorkDays = salary.WorkDays,
                ActualWorkDays = salary.ActualWorkDays,
                LateDays = salary.LateDays,
                EarlyLeaveDays = salary.EarlyLeaveDays,
                BaseSalary = salary.BaseSalary,
                OvertimePay = salary.OvertimePay,
                Bonus = salary.Bonus,
                Allowance = salary.Allowance,
                Deductions = salary.Deductions,
                Insurance = salary.Insurance,
                Tax = salary.Tax,
                NetSalary = salary.NetSalary,
                Status = salary.Status.ToString(),
                StatusName = GetStatusName(salary.Status),
                ApprovedByName = salary.ApprovedByUser?.FullName,
                ApprovedTime = salary.ApprovedTime,
                PaidByName = salary.PaidByUser?.FullName,
                PaidTime = salary.PaidTime,
                Note = salary.Note,
                CreatedTime = salary.CreatedTime
            };
        }

        private int GetWorkDaysInMonth(int year, int month)
        {
            int workDays = 0;
            var date = new DateTime(year, month, 1);
            while (date.Month == month)
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    workDays++;
                date = date.AddDays(1);
            }
            return workDays;
        }

        private decimal CalculateTax(decimal taxableIncome)
        {
            // Simple tax calculation (Vietnam PIT progressive rates)
            if (taxableIncome <= 5000000) return 0;
            if (taxableIncome <= 10000000) return (taxableIncome - 5000000) * 0.05m;
            if (taxableIncome <= 18000000) return 250000 + (taxableIncome - 10000000) * 0.10m;
            if (taxableIncome <= 32000000) return 1050000 + (taxableIncome - 18000000) * 0.15m;
            if (taxableIncome <= 52000000) return 3150000 + (taxableIncome - 32000000) * 0.20m;
            if (taxableIncome <= 80000000) return 7150000 + (taxableIncome - 52000000) * 0.25m;
            return 14150000 + (taxableIncome - 80000000) * 0.30m;
        }

        private string GetStatusName(SalaryStatus status)
        {
            return status switch
            {
                SalaryStatus.Draft => "Nháp",
                SalaryStatus.Pending => "Chờ duyệt",
                SalaryStatus.Approved => "Đã duyệt",
                SalaryStatus.Paid => "Đã phát",
                SalaryStatus.Cancelled => "Đã hủy",
                _ => "N/A"
            };
        }
    }
}
