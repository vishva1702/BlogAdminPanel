namespace BlogAdminPanel.Models
{
    public class User
    {
        public int Id { get; set; } // Use your own Id type (e.g., int)
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin, Editor, etc.
        public bool IsActive { get; set; }

        // Audit Columns
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
    }


}
