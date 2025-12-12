using System.ComponentModel.DataAnnotations;

namespace BaseSource.ViewModels.User
{
    public class ResetPasswordVm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Password minimum 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Password minimum 6 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
