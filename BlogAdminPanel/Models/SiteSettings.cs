namespace BlogAdminPanel.Models
{
    public class SiteSettings
    {
        public int Id { get; set; }
        public string SiteName { get; set; }
        public string Tagline { get; set; }
        public string Logo { get; set; }

        // Contact Info
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string SocialLinks { get; set; }

        // Audit Columns
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }

    }
}
