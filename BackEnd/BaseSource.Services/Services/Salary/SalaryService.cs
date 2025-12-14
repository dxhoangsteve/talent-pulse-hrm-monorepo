using BaseSource.Data.EF;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Salary;
using Microsoft.EntityFrameworkCore;

using BaseSource.Shared.Helpers;

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
            return await CalculateSalaryInternalAsync(adminId, request, saveToDb: true);
        }

        public async Task<ApiResult<SalaryVm>> CalculateSalaryPreviewAsync(string adminId, CalculateSalaryRequest request)
        {
            return await CalculateSalaryInternalAsync(adminId, request, saveToDb: false);
        }

        private async Task<ApiResult<SalaryVm>> CalculateSalaryInternalAsync(string adminId, CalculateSalaryRequest request, bool saveToDb)
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

                if (!saveToDb && existingSalary != null)
                {
                    // If previewing but salary exists, we might want to return the updated calculation or the existing one?
                    // Let's assume preview recalculates based on current attendance.
                }

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
                decimal dailyRate = workDays > 0 ? baseSalary / workDays : 0;
                decimal actualBasePay = dailyRate * actualWorkDays;

                decimal otRate = dailyRate / 8 * 1.5m; // 1.5x hourly rate for OT
                decimal overtimePay = otRate * totalOTHours;

                decimal bonus = request.Bonus ?? 0;
                decimal allowance = request.Allowance ?? 0;
                decimal deductions = request.Deductions ?? 0;

                // User requested to remove deductions ("khấu trừ")
                // decimal autoDeductions = (lateDays + earlyLeaveDays) * 50000;
                // decimal totalDeductions = (request.Deductions ?? 0) + autoDeductions;
                decimal totalDeductions = 0;

                decimal insurance = baseSalary * 0.105m; // 10.5% insurance
                decimal taxableIncome = actualBasePay + overtimePay + bonus + allowance - totalDeductions - insurance;
                decimal tax = CalculateTax(taxableIncome);

                decimal netSalary = actualBasePay + overtimePay + bonus + allowance - totalDeductions - insurance - tax;

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
                salary.Deductions = totalDeductions;
                salary.Insurance = insurance;
                salary.Tax = tax;
                salary.NetSalary = Math.Max(0, netSalary);
                salary.Status = existingSalary?.Status ?? SalaryStatus.Pending; // Keep existing status if exists
                salary.Note = request.Note;
                salary.UpdatedTime = TimeHelper.VietnamNow;

                if (saveToDb)
                {
                    if (existingSalary == null)
                    {
                        _context.Salaries.Add(salary);
                    }
                    await _context.SaveChangesAsync();
                }

                // If not saving, we perform the Map manually since MapToVmAsync uses EF references that might be loose?
                // Actually MapToVmAsync fetches Employee again. 
                // We can just return the mapped object.
                // Note: IF not saving, salary.Id is 0. MapToVmAsync should handle it.

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

        public async Task<ApiResult<PagedResult<SalaryVm>>> GetAllSalaryAsync(int month, int year, Guid? departmentId = null, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Salaries
                    .Include(s => s.Employee)
                        .ThenInclude(e => e.Department)
                    .Include(s => s.ApprovedByUser)
                    .Include(s => s.PaidByUser)
                    .Where(s => s.Month == month && s.Year == year);

                // Filter by department if specified
                if (departmentId.HasValue)
                {
                    query = query.Where(s => s.Employee.DepartmentId == departmentId);
                }

                var totalCount = await query.CountAsync();

                var salaries = await query
                    .OrderBy(s => s.Employee.FullName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = new List<SalaryVm>();
                foreach (var s in salaries)
                {
                    items.Add(await MapToVmAsync(s));
                }

                var pagedResult = new PagedResult<SalaryVm>
                {
                    Items = items,
                    PageIndex = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return new ApiSuccessResult<PagedResult<SalaryVm>>(pagedResult);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<PagedResult<SalaryVm>>(ex.Message);
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
                salary.ApprovedTime = TimeHelper.VietnamNow;
                salary.UpdatedTime = TimeHelper.VietnamNow;

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
                salary.PaidTime = TimeHelper.VietnamNow;
                salary.Note = string.IsNullOrEmpty(note) ? salary.Note : note;
                salary.UpdatedTime = TimeHelper.VietnamNow;

                await _context.SaveChangesAsync();

                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<bool>> ConfirmSalaryAsync(string userId, string salaryId, bool isConfirmed, string? note)
        {
            try
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
                if (employee == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };

                var salary = await _context.Salaries.FindAsync(salaryId);
                if (salary == null) return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy phiếu lương" };

                if (salary.EmployeeId != employee.Id) return new ApiResult<bool> { IsSuccessed = false, Message = "Bạn không có quyền truy cập phiếu lương này" };

                if (salary.Status != SalaryStatus.Paid && salary.Status != SalaryStatus.Complaining)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Chỉ có thể xác nhận phiếu lương đã được thanh toán" };
                }

                if (isConfirmed)
                {
                    salary.Status = SalaryStatus.Confirmed;
                    salary.Note = note; // Feedback note if any
                }
                else
                {
                    salary.Status = SalaryStatus.Complaining;
                    // Note is required for complaining? Maybe handled separately via Complaint ticket, but here we just mark status.
                    if (!string.IsNullOrEmpty(note)) salary.Note = note;
                }

                salary.UpdatedTime = TimeHelper.VietnamNow;
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
                SalaryStatus.Confirmed => "Đã nhận",
                SalaryStatus.Complaining => "Đang khiếu nại",
                SalaryStatus.Cancelled => "Đã hủy",
                _ => "N/A"
            };
        }

        public async Task<ApiResult<bool>> UpdateSalaryAsync(string adminId, string salaryId, UpdateSalaryRequest request)
        {
            try
            {
                var salary = await _context.Salaries.FindAsync(salaryId);
                if (salary == null)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy phiếu lương" };
                }

                if (salary.Status == SalaryStatus.Paid)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Không thể sửa phiếu lương đã thanh toán" };
                }

                if (request.BaseSalary.HasValue) salary.BaseSalary = request.BaseSalary.Value;
                if (request.OvertimePay.HasValue) salary.OvertimePay = request.OvertimePay.Value;
                if (request.Bonus.HasValue) salary.Bonus = request.Bonus.Value;
                if (request.Allowance.HasValue) salary.Allowance = request.Allowance.Value;
                if (request.Deductions.HasValue) salary.Deductions = request.Deductions.Value;
                if (!string.IsNullOrEmpty(request.Note)) salary.Note = request.Note;

                // Recalculate net salary
                decimal taxableIncome = salary.BaseSalary + salary.OvertimePay + salary.Bonus + salary.Allowance - salary.Deductions - salary.Insurance;
                salary.Tax = CalculateTax(taxableIncome);
                salary.NetSalary = Math.Max(0, taxableIncome - salary.Tax);
                salary.UpdatedTime = TimeHelper.VietnamNow;

                await _context.SaveChangesAsync();

                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<ComplaintVm>> CreateComplaintAsync(string employeeId, CreateComplaintRequest request)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.UserId == employeeId);

                if (employee == null)
                {
                    return new ApiResult<ComplaintVm> { IsSuccessed = false, Message = "Không tìm thấy thông tin nhân viên" };
                }

                // Check if complaint already exists for this month
                var existing = await _context.SalaryComplaints
                    .FirstOrDefaultAsync(c => c.EmployeeId == employeeId && c.Month == request.Month && c.Year == request.Year && c.Status == ComplaintStatus.Pending);

                if (existing != null)
                {
                    return new ApiResult<ComplaintVm> { IsSuccessed = false, Message = "Bạn đã có khiếu nại đang chờ xử lý cho tháng này" };
                }

                var complaint = new Data.Entities.SalaryComplaint
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    Month = request.Month,
                    Year = request.Year,
                    ComplaintType = request.ComplaintType,
                    Content = request.Content,
                    SalarySlipId = request.SalarySlipId,
                    Status = ComplaintStatus.Pending,
                    CreatedTime = TimeHelper.VietnamNow
                };

                _context.SalaryComplaints.Add(complaint);
                await _context.SaveChangesAsync();

                return new ApiResult<ComplaintVm>
                {
                    IsSuccessed = true,
                    ResultObj = MapComplaintToVm(complaint, employee.FullName, employee.Department?.Name)
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<ComplaintVm> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<ComplaintVm>>> GetMyComplaintsAsync(string employeeId)
        {
            try
            {
                var complaints = await _context.SalaryComplaints
                    .Include(c => c.Employee)
                        .ThenInclude(e => e!.Department)
                    .Include(c => c.ResolvedBy)
                    .Where(c => c.EmployeeId == employeeId)
                    .OrderByDescending(c => c.CreatedTime)
                    .ToListAsync();

                var result = complaints.Select(c => MapComplaintToVm(
                    c,
                    c.Employee?.FullName ?? "N/A",
                    c.Employee?.Department?.Name,
                    c.ResolvedBy?.FullName
                )).ToList();

                return new ApiResult<List<ComplaintVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<ComplaintVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<List<ComplaintVm>>> GetAllComplaintsAsync()
        {
            try
            {
                var complaints = await _context.SalaryComplaints
                    .Include(c => c.Employee)
                        .ThenInclude(e => e!.Department)
                    .Include(c => c.ResolvedBy)
                    .OrderByDescending(c => c.CreatedTime)
                    .ToListAsync();

                var result = complaints.Select(c => MapComplaintToVm(
                    c,
                    c.Employee?.FullName ?? "N/A",
                    c.Employee?.Department?.Name,
                    c.ResolvedBy?.FullName
                )).ToList();

                return new ApiResult<List<ComplaintVm>> { IsSuccessed = true, ResultObj = result };
            }
            catch (Exception ex)
            {
                return new ApiResult<List<ComplaintVm>> { IsSuccessed = false, Message = ex.Message };
            }
        }

        public async Task<ApiResult<bool>> ResolveComplaintAsync(string adminId, Guid complaintId, ResolveComplaintRequest request)
        {
            try
            {
                var complaint = await _context.SalaryComplaints.FindAsync(complaintId);
                if (complaint == null)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Không tìm thấy khiếu nại" };
                }

                if (complaint.Status != ComplaintStatus.Pending && complaint.Status != ComplaintStatus.InProgress)
                {
                    return new ApiResult<bool> { IsSuccessed = false, Message = "Khiếu nại đã được xử lý" };
                }

                complaint.Status = request.Status;
                complaint.Response = request.Response;
                complaint.ResolvedById = adminId;
                complaint.ResolvedTime = TimeHelper.VietnamNow;

                await _context.SaveChangesAsync();

                return new ApiResult<bool> { IsSuccessed = true, ResultObj = true };
            }
            catch (Exception ex)
            {
                return new ApiResult<bool> { IsSuccessed = false, Message = ex.Message };
            }
        }

        private ComplaintVm MapComplaintToVm(Data.Entities.SalaryComplaint c, string employeeName, string? departmentName, string? resolvedByName = null)
        {
            return new ComplaintVm
            {
                Id = c.Id,
                EmployeeId = c.EmployeeId,
                EmployeeName = employeeName,
                DepartmentName = departmentName,
                Month = c.Month,
                Year = c.Year,
                ComplaintType = c.ComplaintType,
                ComplaintTypeName = GetComplaintTypeName(c.ComplaintType),
                Content = c.Content,
                Status = c.Status,
                StatusName = GetComplaintStatusName(c.Status),
                ResolvedByName = resolvedByName,
                Response = c.Response,
                ResolvedTime = c.ResolvedTime,
                CreatedTime = c.CreatedTime
            };
        }

        private string GetComplaintTypeName(ComplaintType type)
        {
            return type switch
            {
                ComplaintType.NotPaid => "Chưa nhận lương",
                ComplaintType.WrongAmount => "Sai số tiền",
                ComplaintType.MissingOT => "Thiếu tiền OT",
                ComplaintType.MissingBonus => "Thiếu tiền thưởng",
                ComplaintType.Other => "Khác",
                _ => "N/A"
            };
        }

        private string GetComplaintStatusName(ComplaintStatus status)
        {
            return status switch
            {
                ComplaintStatus.Pending => "Chờ xử lý",
                ComplaintStatus.InProgress => "Đang xử lý",
                ComplaintStatus.Resolved => "Đã xử lý",
                ComplaintStatus.Rejected => "Từ chối",
                _ => "N/A"
            };
        }
    }
}
