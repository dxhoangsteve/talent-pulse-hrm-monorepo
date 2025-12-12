using BaseSource.ViewModels.UserAdmin;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.UserAdmin
{
    public interface IUserAdminService
    {
        Task<IPagedList<UserAdminInfoDto>> GetUserByFilterAsync(UserAdminRequestDto model);
        Task<KeyValuePair<bool, string>> UpdateManagerAsync(string userName);
        Task<List<UserAdminInfoDto>> GetUserManagerAsync();
        
        Task<KeyValuePair<bool, string>> ResetPassowrdAsync(ResetPasswordAdminDto model);
        
        Task<KeyValuePair<bool,string>> DeleteAsync(string userId);
        Task<KeyValuePair<bool, string>> CreateUserAsync(UserCreateAdminDto model);


    }
}
