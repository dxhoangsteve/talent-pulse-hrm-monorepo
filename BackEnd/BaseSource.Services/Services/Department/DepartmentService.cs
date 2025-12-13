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
    }
}

