using System.ComponentModel.DataAnnotations;

namespace BlogAdminPanel.Validation
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        public PasswordValidationAttribute()
            : base("Password must be between 8 and 16 characters, include at least one uppercase letter, one number, and one special character.")
        {
        }

        public override bool IsValid(object value)
        {
            var password = value as string;

            // Check if the password is not null
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Password Length check
            if (password.Length < 8 || password.Length > 16)
            {
                return false;
            }

            // Uppercase letter check
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // Number check
            if (!password.Any(char.IsDigit))
            {
                return false;
            }

            // Special character check
            if (!password.Any(ch => "!@#$%^&*(),.?\":{}|<>".Contains(ch)))
            {
                return false;
            }

            return true;
        }
    }
}