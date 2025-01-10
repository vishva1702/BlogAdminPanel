using BlogAdminPanel.Validation;

namespace BlogAdminPanel.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Editor
        public bool IsActive { get; set; }

        // Audit Columns
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; } 
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
