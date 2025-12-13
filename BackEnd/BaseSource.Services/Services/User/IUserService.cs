using BaseSource.ViewModels.User;
using BaseSource.ViewModels.Common;

namespace BaseSouce.Services.Services.User
{
    public interface IUserService
    {
        Task<UserInfoResponse> GetUserInfoAsync(string id);
        Task<KeyValuePair<bool, string>> CreateAsync(RegisterRequestVm model);
        Task<ApiResult<LoginResponseVm>> AuthenticateAsync(LoginRequestVm model);
        Task<KeyValuePair<bool, string>> ConfirmEmailAsync(ConfirmEmailVm model);
        Task<KeyValuePair<bool, string>> ForgotPasswordAsync(ForgotPasswordVm model);
        Task<KeyValuePair<bool, string>> ResetPasswordAsync(ResetPasswordVm model);
        Task<KeyValuePair<bool, string>> ChangePasswordAsync(string id, ChangePasswordVm model);
        Task<ProfifleInfoDto> GetProfileInfoAsync(string userId);
        Task<KeyValuePair<bool, string>> UpdateAsync(string userId, UserUpdateDto model);
        Task<KeyValuePair<bool, string>> DeleteAsync(string userId);
        Task<KeyValuePair<bool, string>> SetPasswordAsync(string userId, string newPassword);
        Task<ApiResult<PagedResult<UserVm>>> GetUsersPagingAsync(GetUserPagingRequest request);


    }
}
