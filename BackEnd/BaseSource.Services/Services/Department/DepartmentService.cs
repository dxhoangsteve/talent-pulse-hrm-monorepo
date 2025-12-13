using AutoMapper;
using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Enums;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.Department;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseSource.Services.Services.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly BaseSourceDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DepartmentService(BaseSourceDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApiResult<bool>> CreateAsync(CreateDepartmentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (request.ManagerId == request.DeputyId)
                {
                    return new ApiErrorResult<bool>("Trưởng phòng và Phó phòng không được là cùng một người.");
                }

                var manager = await _userManager.FindByIdAsync(request.ManagerId);
                if (manager == null) return new ApiErrorResult<bool>("Trưởng phòng không tồn tại.");

                var deputy = await _userManager.FindByIdAsync(request.DeputyId);
                if (deputy == null) return new ApiErrorResult<bool>("Phó phòng không tồn tại.");

                // Create Department
                var department = new BaseSource.Data.Entities.Department
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    ManagerId = request.ManagerId,
                    DeputyId = request.DeputyId,
                    CreatedTime = DateTime.UtcNow
                };

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                // Update Manager
                manager.DepartmentId = department.Id;
                manager.Position = PositionType.Manager;
                await _userManager.UpdateAsync(manager);

                // Update Deputy
                deputy.DepartmentId = department.Id;
                deputy.Position = PositionType.DeputyManager;
                await _userManager.UpdateAsync(deputy);

                await transaction.CommitAsync();
                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<bool>($"Lỗi khi tạo phòng ban: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<DepartmentVm>>> GetAllAsync()
        {
            var depts = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Manager)
                .Include(d => d.Deputy)
                .Include(d => d.Users)
                .Select(d => new DepartmentVm
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    ManagerId = d.ManagerId,
                    ManagerName = d.Manager != null ? (d.Manager.FullName ?? d.Manager.UserName) : "Chưa có",
                    DeputyId = d.DeputyId,
                    DeputyName = d.Deputy != null ? (d.Deputy.FullName ?? d.Deputy.UserName) : "Chưa có",
                    UserCount = d.Users.Count
                }).ToListAsync();

            return new ApiSuccessResult<List<DepartmentVm>>(depts);
        }

        public async Task<ApiResult<bool>> UpdateLeadershipAsync(Guid departmentId, UpdateDepartmentLeadershipVm model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var department = await _context.Departments
                    .Include(d => d.Manager)
                    .Include(d => d.Deputy)
                    .FirstOrDefaultAsync(d => d.Id == departmentId);

                if (department == null)
                {
                    return new ApiErrorResult<bool>("Phòng ban không tồn tại");
                }

                // Validate: Manager and Deputy cannot be the same
                if (model.ManagerId != null && model.DeputyId != null && model.ManagerId == model.DeputyId)
                {
                    return new ApiErrorResult<bool>("Trưởng phòng và Phó phòng không được là cùng một người");
                }

                // Validate: New manager cannot already be manager of another department
                if (!string.IsNullOrEmpty(model.ManagerId) && model.ManagerId != department.ManagerId)
                {
                    var existingManagedDept = await _context.Departments
                        .FirstOrDefaultAsync(d => d.ManagerId == model.ManagerId && d.Id != departmentId);
                    if (existingManagedDept != null)
                    {
                        return new ApiErrorResult<bool>("Người này đã là Trưởng phòng của phòng ban khác");
                    }
                }

                // Validate: New deputy cannot already be deputy of another department
                if (!string.IsNullOrEmpty(model.DeputyId) && model.DeputyId != department.DeputyId)
                {
                    var existingDeputyDept = await _context.Departments
                        .FirstOrDefaultAsync(d => d.DeputyId == model.DeputyId && d.Id != departmentId);
                    if (existingDeputyDept != null)
                    {
                        return new ApiErrorResult<bool>("Người này đã là Phó phòng của phòng ban khác");
                    }
                }

                // Update old manager's position
                if (department.Manager != null && department.ManagerId != model.ManagerId)
                {
                    department.Manager.Position = PositionType.None;
                    await _userManager.UpdateAsync(department.Manager);
                }

                // Update old deputy's position
                if (department.Deputy != null && department.DeputyId != model.DeputyId)
                {
                    department.Deputy.Position = PositionType.None;
                    await _userManager.UpdateAsync(department.Deputy);
                }

                // Set new manager
                if (!string.IsNullOrEmpty(model.ManagerId))
                {
                    var newManager = await _userManager.FindByIdAsync(model.ManagerId);
                    if (newManager == null)
                    {
                        return new ApiErrorResult<bool>("Trưởng phòng mới không tồn tại");
                    }
                    newManager.DepartmentId = departmentId;
                    newManager.Position = PositionType.Manager;
                    await _userManager.UpdateAsync(newManager);
                    department.ManagerId = model.ManagerId;
                }
                else
                {
                    department.ManagerId = null;
                }

                // Set new deputy
                if (!string.IsNullOrEmpty(model.DeputyId))
                {
                    var newDeputy = await _userManager.FindByIdAsync(model.DeputyId);
                    if (newDeputy == null)
                    {
                        return new ApiErrorResult<bool>("Phó phòng mới không tồn tại");
                    }
                    newDeputy.DepartmentId = departmentId;
                    newDeputy.Position = PositionType.DeputyManager;
                    await _userManager.UpdateAsync(newDeputy);
                    department.DeputyId = model.DeputyId;
                }
                else
                {
                    department.DeputyId = null;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<bool>($"Lỗi khi cập nhật lãnh đạo phòng ban: {ex.Message}");
            }
        }

        public async Task<ApiResult<List<UserSelectVm>>> GetAvailableLeadersAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.IsActive)
                .Include(u => u.Department)
                .OrderBy(u => u.FullName)
                .Select(u => new UserSelectVm
                {
                    Id = u.Id,
                    FullName = u.FullName ?? u.UserName ?? "N/A",
                    Email = u.Email,
                    CurrentDepartment = u.Department != null ? u.Department.Name : null
                })
                .ToListAsync();

            return new ApiSuccessResult<List<UserSelectVm>>(users);
        }

        /// <summary>
        /// Lấy phòng ban mà user đang quản lý (Manager hoặc Deputy)
        /// </summary>
        public async Task<Guid?> GetManagedDepartmentIdAsync(string userId)
        {
            // Check if user is Manager of any department
            var deptAsManager = await _context.Departments
                .FirstOrDefaultAsync(d => d.ManagerId == userId);
            
            if (deptAsManager != null)
                return deptAsManager.Id;

            // Check if user is Deputy of any department
            var deptAsDeputy = await _context.Departments
                .FirstOrDefaultAsync(d => d.DeputyId == userId);
            
            if (deptAsDeputy != null)
                return deptAsDeputy.Id;

            return null;
        }

        /// <summary>
        /// Lấy danh sách nhân viên trong phòng ban
        /// </summary>
        public async Task<ApiResult<List<UserSelectVm>>> GetDepartmentEmployeesAsync(Guid departmentId)
        {
            var employees = await _context.Users
                .AsNoTracking()
                .Where(u => u.DepartmentId == departmentId && u.IsActive)
                .OrderBy(u => u.FullName)
                .Select(u => new UserSelectVm
                {
                    Id = u.Id,
                    FullName = u.FullName ?? u.UserName ?? "N/A",
                    Email = u.Email,
                    CurrentDepartment = null
                })
                .ToListAsync();

            return new ApiSuccessResult<List<UserSelectVm>>(employees);
        }

        /// <summary>
        /// Thêm nhân viên vào phòng ban
        /// </summary>
        public async Task<ApiResult<bool>> AddEmployeeToDepartmentAsync(Guid departmentId, string employeeId)
        {
            try
            {
                var department = await _context.Departments.FindAsync(departmentId);
                if (department == null)
                {
                    return new ApiErrorResult<bool>("Phòng ban không tồn tại");
                }

                var user = await _userManager.FindByIdAsync(employeeId);
                if (user == null)
                {
                    return new ApiErrorResult<bool>("Nhân viên không tồn tại");
                }

                // Check if user is already in another department
                if (user.DepartmentId != null && user.DepartmentId != departmentId)
                {
                    var oldDept = await _context.Departments.FindAsync(user.DepartmentId);
                    return new ApiErrorResult<bool>($"Nhân viên đã thuộc phòng ban {oldDept?.Name}. Vui lòng xóa khỏi phòng ban cũ trước.");
                }

                user.DepartmentId = departmentId;
                await _userManager.UpdateAsync(user);

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>($"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa nhân viên khỏi phòng ban
        /// </summary>
        public async Task<ApiResult<bool>> RemoveEmployeeFromDepartmentAsync(Guid departmentId, string employeeId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(employeeId);
                if (user == null)
                {
                    return new ApiErrorResult<bool>("Nhân viên không tồn tại");
                }

                if (user.DepartmentId != departmentId)
                {
                    return new ApiErrorResult<bool>("Nhân viên không thuộc phòng ban này");
                }

                // Check if user is manager or deputy
                var dept = await _context.Departments.FindAsync(departmentId);
                if (dept != null)
                {
                    if (dept.ManagerId == employeeId)
                    {
                        return new ApiErrorResult<bool>("Không thể xóa Trưởng phòng. Vui lòng thay đổi Trưởng phòng trước.");
                    }
                    if (dept.DeputyId == employeeId)
                    {
                        return new ApiErrorResult<bool>("Không thể xóa Phó phòng. Vui lòng thay đổi Phó phòng trước.");
                    }
                }

                user.DepartmentId = null;
                user.Position = PositionType.None;
                await _userManager.UpdateAsync(user);

                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>($"Lỗi: {ex.Message}");
            }
        }
    }
}
