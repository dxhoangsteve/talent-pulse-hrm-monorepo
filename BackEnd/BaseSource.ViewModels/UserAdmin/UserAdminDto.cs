using BaseSource.ViewModels.Common;

namespace BaseSource.ViewModels.UserAdmin
{
    public class UserAdminInfoDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string LinkFB { get; set; }
        public string LinkTelegram { get; set; }
        public DateTime CreatedTime { get; set; }
        public string TelegramAPI { get; set; }
        public string Password { get; set; }
        public int TotalBOT { get; set; }
        public int TotalChannel { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UserUpdate { get; set; }
    }
    public class UserAdminRequestDto : PageQuery
    {
        public string UserName { get; set; }
        public bool IsOrder { get; set; }
    }
    public class ResetPasswordAdminDto
    {
        public string UserUpdate { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }
    public class UserCreateAdminDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int NumberOfThreads1080 { get; set; }
        public int NumberOfThreads4K { get; set; }
        public string LinkTelegram { get; set; }
    }
}
