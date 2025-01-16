using BlogAdminPanel.Validation;

namespace BlogAdminPanel.Models.DTOs
{
    public class UserCreateDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        [PasswordValidation]
        public string PasswordHash { get; set; }
        public string Role { get; set; } 
        public bool IsActive { get; set; }
        
    }

    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }

    public class SiteSettingsDto
    {
        public string SiteName { get; set; }
        public string Tagline { get; set; }
        public IFormFile? LogoFile { get; set; }
        public string? Logo { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string SocialLinks { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
