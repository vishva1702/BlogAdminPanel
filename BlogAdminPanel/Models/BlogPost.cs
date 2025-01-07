namespace BlogAdminPanel.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public string Status { get; set; } // Draft, Published, Archived
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Comment> Comments { get; set; }
    }
}
