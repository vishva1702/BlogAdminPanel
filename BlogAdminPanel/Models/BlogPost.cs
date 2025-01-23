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
        public string Status { get; set; }
        public bool IsDraft { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? PublishedDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int CategoryId { get; set; } 
        public int TagId { get; set; }
        public string Image { get; set; }
        public int Views { get; set; } = 0;
        public int Likes { get; set; } = 0;
        public int Shares { get; set; } = 0;
        public Category Category { get; set; }
        public Tag Tag { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}