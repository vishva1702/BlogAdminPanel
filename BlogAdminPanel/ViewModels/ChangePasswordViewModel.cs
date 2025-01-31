using BlogAdminPanel.Validation;
using System.ComponentModel.DataAnnotations;

namespace BlogAdminPanel.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
        [DataType(DataType.Password)] // Marks as a password field
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
        [DataType(DataType.Password)] // Marks as a password field
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)] // Marks as a password field
        [Compare("NewPassword", ErrorMessage = "New password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}