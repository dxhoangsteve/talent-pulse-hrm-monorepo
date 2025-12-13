using BaseSouce.Services.Services.User;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userServices;


        public AccountController(IUserService userServices)
        {
            _userServices = userServices;
        }



        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var result = await _userServices.CreateAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>());
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost("Authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequestVm user)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _userServices.AuthenticateAsync(user);
            if (result.IsSuccessed)
            {
                return Ok(result);
            }
            return Ok(result);
        }

        [HttpPost("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailVm model)
        {

            var result = await _userServices.ConfirmEmailAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>());
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _userServices.ForgotPasswordAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>());
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVm model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _userServices.ResetPasswordAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>());
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _userServices.GetUserInfoAsync(UserId);
            return Ok(new ApiSuccessResult<UserInfoResponse>(result));
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVm model)
        {
            var result = await _userServices.ChangePasswordAsync(UserId, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>());
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfileInfo()
        {
            var result = await _userServices.GetProfileInfoAsync(UserId);
            return Ok(new ApiSuccessResult<ProfifleInfoDto>(result));
        }

       

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UserUpdateDto model)
        {
            var result = await _userServices.UpdateAsync(UserId, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>());
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUserPagingRequest request)
        {
            var result = await _userServices.GetUsersPagingAsync(request);
            return Ok(result);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto model)
        {
            var result = await _userServices.UpdateAsync(id, model);
            if (result.Key) return Ok(new ApiSuccessResult<string>());
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userServices.DeleteAsync(id);
            if (result.Key) return Ok(new ApiSuccessResult<string>());
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost("users/{id}/set-password")]
        public async Task<IActionResult> SetPassword(string id, [FromBody] string newPassword)
        {
            var result = await _userServices.SetPasswordAsync(id, newPassword);
            if (result.Key) return Ok(new ApiSuccessResult<string>());
            return Ok(new ApiErrorResult<string>(result.Value));
        }

    }
}
