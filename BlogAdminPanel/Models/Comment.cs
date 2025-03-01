﻿namespace BlogAdminPanel.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsApproved { get; set; }
        public int BlogPostId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public BlogPost BlogPost { get; set; }
    }
}
