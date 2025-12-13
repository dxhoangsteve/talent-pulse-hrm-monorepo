using AutoMapper;
using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BaseSouce.Services.Services.User
{
    public class UserService : IUserService
    {
        private readonly BaseSourceDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UserService(
            BaseSourceDbContext context,
            UserManager<AppUser> userManager,
            ILogger<UserService> logger,
            IMapper mapper,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<ApiResult<LoginResponseVm>> AuthenticateAsync(LoginRequestVm model)
        {
            try
            {
                var userName = model.UserName.ToLower();
                var existingUser = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => 
                        (x.UserName!.ToLower() == userName || x.Email!.ToLower() == userName) 
                        && x.IsActive);

                if (existingUser == null)
                {
                    return new ApiErrorResult<LoginResponseVm>("Tài khoản không tồn tại");
                }

                var result = await _signInManager.PasswordSignInAsync(existingUser, model.Password, true, true);
                
                if (!result.Succeeded)
                {
                    var error = result.IsLockedOut 
                        ? "Tài khoản đã bị khóa." 
                        : "Tài khoản hoặc mật khẩu không chính xác!";
                    return new ApiErrorResult<LoginResponseVm>(error);
                }

                var token = await GenerateJwtToken(existingUser);
                var roles = await _userManager.GetRolesAsync(existingUser);
                var role = roles.FirstOrDefault() ?? "Employee";

                return new ApiSuccessResult<LoginResponseVm>(new LoginResponseVm 
                { 
                    Token = token,
                    Role = role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication failed");
                return new ApiErrorResult<LoginResponseVm>("Đăng nhập thất bại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> CreateAsync(RegisterRequestVm model)
        {
            try
            {
                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    return new KeyValuePair<bool, string>(false, "Tài khoản đã tồn tại!");
                }

                if (await _userManager.FindByEmailAsync(model.UserName) != null)
                {
                    return new KeyValuePair<bool, string>(false, "Email đã được sử dụng!");
                }

                var user = new AppUser
                {
                    Email = model.UserName,
                    UserName = model.UserName,
                    NormalizedEmail = model.UserName.ToUpper(),
                    NormalizedUserName = model.UserName.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FullName = model.UserName,
                    IsActive = true,
                    CreatedTime = DateTime.UtcNow,
                    BaseSalary = model.BaseSalary,
                    DepartmentId = model.DepartmentId,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new KeyValuePair<bool, string>(false, errors);
                }

                // Add default Employee role
                await _userManager.AddToRoleAsync(user, "Employee");

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create account failed");
                return new KeyValuePair<bool, string>(false, "Tạo tài khoản thất bại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> ConfirmEmailAsync(ConfirmEmailVm model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return new KeyValuePair<bool, string>(false, "Tài khoản không tồn tại");

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            
            return result.Succeeded
                ? new KeyValuePair<bool, string>(true, string.Empty)
                : new KeyValuePair<bool, string>(false, "Xác thực email thất bại!");
        }

        public async Task<KeyValuePair<bool, string>> ForgotPasswordAsync(ForgotPasswordVm model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email!.ToLower() == model.Email.ToLower());

            if (user == null)
                return new KeyValuePair<bool, string>(true, string.Empty); // Don't reveal user existence

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var clientEndpoint = _configuration["ClientEndpoint"];
            var callbackUrl = $"{clientEndpoint}/auth/reset-password?code={encodedToken}&email={model.Email}";

            // TODO: Send email with callbackUrl
            _logger.LogInformation("Password reset link: {Url}", callbackUrl);

            return new KeyValuePair<bool, string>(true, string.Empty);
        }

        public async Task<KeyValuePair<bool, string>> ResetPasswordAsync(ResetPasswordVm model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new KeyValuePair<bool, string>(false, "Email không tồn tại");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

            return result.Succeeded
                ? new KeyValuePair<bool, string>(true, string.Empty)
                : new KeyValuePair<bool, string>(false, "Đặt lại mật khẩu thất bại!");
        }

        public async Task<KeyValuePair<bool, string>> ChangePasswordAsync(string id, ChangePasswordVm model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    return new KeyValuePair<bool, string>(false, "Tài khoản không tồn tại");

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return new KeyValuePair<bool, string>(true, string.Empty);
                }

                return new KeyValuePair<bool, string>(false, "Mật khẩu cũ không chính xác!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change password failed");
                return new KeyValuePair<bool, string>(false, "Đổi mật khẩu thất bại!");
            }
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UserInfoResponse>(user);
        }

        public async Task<ProfifleInfoDto> GetProfileInfoAsync(string userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => new ProfifleInfoDto
                {
                    UserName = x.UserName,
                    Phone = x.PhoneNumber,
                })
                .FirstOrDefaultAsync();

            return user ?? new ProfifleInfoDto();
        }

        public async Task<KeyValuePair<bool, string>> UpdateAsync(string userId, UserUpdateDto model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");

                user.PhoneNumber = model.Phone ?? user.PhoneNumber;
                user.Email = model.Email ?? user.Email;

                await _context.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update user failed");
                return new KeyValuePair<bool, string>(false, "Cập nhật không thành công!");
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
            {
                return new KeyValuePair<bool, string>(false, "Không thể xóa tài khoản Quản trị tối cao (SuperAdmin)!");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded) return new KeyValuePair<bool, string>(false, "Xóa thất bại");

            return new KeyValuePair<bool, string>(true, string.Empty);
        }

        public async Task<KeyValuePair<bool, string>> SetPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại");

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded) return new KeyValuePair<bool, string>(false, "Lỗi khi xóa mật khẩu cũ");

            var addResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addResult.Succeeded) return new KeyValuePair<bool, string>(false, "Lỗi khi đặt mật khẩu mới");

            return new KeyValuePair<bool, string>(true, string.Empty);
        }

        public async Task<ApiResult<PagedResult<UserVm>>> GetUsersPagingAsync(GetUserPagingRequest request)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.UserName.Contains(request.Keyword) || x.Email.Contains(request.Keyword));
            }

            // Paging
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x => x.CreatedTime)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserVm()
                {
                    Email = x.Email,
                    FullName = x.FullName,
                    Id = x.Id,
                    PhoneNumber = x.PhoneNumber,
                    UserName = x.UserName,
                    IsActive = x.IsActive
                }).ToListAsync();

            // Get Roles for each user
            foreach (var user in data)
            {
                var appUser = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(appUser);
                user.Roles = roles;
            }

            var pagedResult = new PagedResult<UserVm>()
            {
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling(totalRow / (double)request.PageSize),
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<UserVm>>(pagedResult);
        }

        #region Private Methods
        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, string.Join(";", roles)),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Tokens:Issuer"],
                audience: _configuration["Tokens:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
