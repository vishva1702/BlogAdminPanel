using BlogAdminPanel.Models.BlogAdminPanel.Models;

namespace BlogAdminPanel.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    }
}
