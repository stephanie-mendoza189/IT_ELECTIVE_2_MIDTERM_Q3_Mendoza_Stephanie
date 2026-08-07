using System.ComponentModel.DataAnnotations;

namespace MvcAuthDemo.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}