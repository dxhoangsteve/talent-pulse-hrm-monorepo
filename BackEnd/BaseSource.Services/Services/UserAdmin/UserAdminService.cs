using BaseSource.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.Shared.Helpers;
using BaseSource.ViewModels.UserAdmin;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.Services.UserAdmin
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<UserAdminService> _logger;
        public UserAdminService(IUnitOfWork<BaseSourceDbContext> unitOfWork,
             RoleManager<AppRole> roleManager, ILogger<UserAdminService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _logger = logger;


        }

        public async Task<KeyValuePair<bool, string>> CreateUserAsync(UserCreateAdminDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<AppUser>();

                var existUser = await _repository.ExistsAsync(x => x.UserName.ToLower() == model.UserName.ToLower());
                if (existUser)
                {
                    return new KeyValuePair<bool, string>(false, "Tài khoản đã tồn tại!");
                }


                var userIdParent = string.Empty;

                var hasher = new PasswordHasher<AppUser>();

                var user = new AppUser
                {
                    Email = model.UserName,
                    UserName = model.UserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "123456789",
                    PasswordHash = hasher.HashPassword(null, model.Password),
                    Password = model.Password,
                    CreatedTime = DateTime.Now,
                    NormalizedEmail = model.UserName,
                    NormalizedUserName = model.UserName,
                    LinkFB = string.Empty,
                    TelegramAPI = model.LinkTelegram,
                    //NumberOfThreads1080 = model.NumberOfThreads1080,
                    //NumberOfThreads4K = model.NumberOfThreads4K

                };
                await _unitOfWork.GetRepository<AppUser>().InsertAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create account failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(true, "Create account failed");
            }
        }


        public async Task<KeyValuePair<bool, string>> DeleteAsync(string userId)
        {
            try
            {
                //var _repository = _unitOfWork.GetRepository<AppUser>();
                ////var _repositoryUserBOT = _unitOfWork.GetRepository<UserManagerBOT>();

                ////var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                //// var _repositoryUserBOT = _unitOfWork.GetRepository<UserManagerBOT>();

                //var userCurrent = await _repository.GetFirstOrDefaultAsync(
                //    predicate: x => x.Id == userId, disableTracking: false);
                //if (userCurrent == null)
                //{
                //    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");
                //}
                //userCurrent.DeletedTime = DateTime.Now;
                ////remove usermanagerBot

                //var userBots = await _repositoryUserBOT.Queryable().Where(x => x.UserId == userId).ToListAsync();
                //if (userBots.Any())
                //{
                //    _repositoryUserBOT.Delete(userBots);
                //}
                //var streams = await _repositoryStream.Queryable()
                //    .Where(x => x.UserId == userId && x.Status == StreamStatus.Streaming).ToListAsync();
                //if (streams.Any())
                //{
                //    foreach (var item in streams)
                //    {
                //        await _streamClientService.CancelStreamAsync(userId, item.Id);
                //    }
                //}

                //// userCurrent.UserManagerBOTs = null;
                ////userCurrent.NumberOfThreads4K = 0;
                ////userCurrent.UserManagerBOTs = null;
                //_repository.Update(userCurrent);

                //await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete User failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Xóa người dùng không thành công:[{ex.Message}]");
            }
        }


        public async Task<IPagedList<UserAdminInfoDto>> GetUserByFilterAsync(UserAdminRequestDto model)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var query = _repository.Queryable().AsNoTracking();
            query = query.Where(x => x.DeletedTime == null);
            if (!string.IsNullOrEmpty(model.UserName))
            {
                query = query.Where(x => x.UserName.Contains(model.UserName) || x.Email.Contains(model.UserName));
            }

            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => new UserAdminInfoDto
                {
                    CreatedTime = x.CreatedTime,
                    Email = x.Email,
                    Id = x.Id,
                    LinkFB = x.LinkFB,
                    LinkTelegram = x.LinkTelegram,
                    Password = x.Password,
                    //TotalChannel=x
                    UpdatedTime = x.UpdatedTime,
                    UserUpdate = x.UserUpdate,
                    UserName = x.UserName,
                    
                },
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);
        }

        public async Task<List<UserAdminInfoDto>> GetUserManagerAsync()
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();
            var roleManager = await _roleManager.FindByNameAsync("Admin");

            var query = _repository.Queryable().AsNoTracking();

            query = query.Where(x => x.DeletedTime == null &&
            x.UserRoles.Any(i => i.RoleId == roleManager.Id));

            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => new UserAdminInfoDto
                {
                    CreatedTime = x.CreatedTime,
                    Email = x.Email,
                    Id = x.Id,
                    LinkFB = x.LinkFB,
                    LinkTelegram = x.LinkTelegram,

                    UserName = x.UserName,
                    TelegramAPI = x.TelegramAPI
                },
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: 1, pageSize: 1000);
            return data.Items.ToList();
        }

        public async Task<KeyValuePair<bool, string>> ResetPassowrdAsync(ResetPasswordAdminDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<AppUser>();

                var userCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.UserId, disableTracking: false);
                if (userCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    model.Password = RandomHelper.RandomString(6);
                }
                var hasher = new PasswordHasher<AppUser>();

                var passwordHash = hasher.HashPassword(null, model.Password);

                userCurrent.Password = model.Password;
                userCurrent.PasswordHash = passwordHash;
                userCurrent.UpdatedTime = DateTime.Now;
                userCurrent.UserUpdate = model.UserUpdate;

                _repository.Update(userCurrent);

                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Resetpassword failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Reset password không thành công, vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateManagerAsync(string userName)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var roleManager = await _roleManager.FindByNameAsync("Admin");

            var userCurrent = await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.UserName == userName,
                include: x => x.Include(i => i.UserRoles),
                disableTracking: false);

            if (userCurrent != null)
            {
                var isRoleCurrent = userCurrent.UserRoles.FirstOrDefault(x => x.RoleId == roleManager.Id);
                if (isRoleCurrent != null)
                {
                    userCurrent.UserRoles.Remove(isRoleCurrent);
                }
                else
                {
                    userCurrent.UserRoles.Add(new AppUserRole
                    {
                        RoleId = roleManager.Id,
                        UserId = userCurrent.Id,
                    });
                }

                _repository.Update(userCurrent);

                await _unitOfWork.SaveChangesAsync();
            }
            return new KeyValuePair<bool, string>(true, string.Empty);
        }
    }
}
