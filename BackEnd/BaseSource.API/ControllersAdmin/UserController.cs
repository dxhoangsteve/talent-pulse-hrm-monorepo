using BaseSouce.Services.Services.UserAdmin;
using BaseSource.ViewModels.Common;
using BaseSource.ViewModels.UserAdmin;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    [Route("/api/admin/[controller]")]
    public class UserController : BaseAdminApiController
    {
        private readonly IUserAdminService _userAdminService;
        public UserController(IUserAdminService userAdminService)
        {
            _userAdminService = userAdminService;
        }

        [HttpGet]
        [Route("/api/admin/users")]
        public async Task<IActionResult> GetAll([FromQuery] UserAdminRequestDto model)
        {
            var result = await _userAdminService.GetUserByFilterAsync(model);
            return Ok(new ApiSuccessResult<object>(result));
        }

        

        

        [HttpGet]
        [Route("managers")]
        public async Task<IActionResult> GetAllManager()
        {
            var result = await _userAdminService.GetUserManagerAsync();
            return Ok(new ApiSuccessResult<object>(result));
        }

       

        [HttpPost]
        [Route("manager/role")]
        public async Task<IActionResult> UpdateRoleManager(string userName)
        {
            var result = await _userAdminService.UpdateManagerAsync(userName);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordAdminDto model)
        {
            model.UserUpdate = User.Identity?.Name;
            var result = await _userAdminService.ResetPassowrdAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost]
        [Route("/api/admin/user/delete")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userAdminService.DeleteAsync(userId);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }
        [HttpPost]
        [Route("/api/admin/user/create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateAdminDto model)
        {
            var result = await _userAdminService.CreateUserAsync(model);
            if (result.Key)
            {
                return Ok(new ApiSuccessResult<string>(string.Empty));
            }
            return Ok(new ApiErrorResult<string>(result.Value));
        }
    }
}
