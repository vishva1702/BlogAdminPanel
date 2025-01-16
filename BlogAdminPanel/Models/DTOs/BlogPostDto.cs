using System.ComponentModel.DataAnnotations;

namespace BlogAdminPanel.Models.DTOs
{
    public class BlogPostCreateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public string Status { get; set; } // Draft, Published, Archived
        public bool IsDraft { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public string CreatedBy { get; set; }
        public int CategoryId { get; set; }
        public int TagId { get; set; }
        public IFormFile ImageFile { get; set; }
        public DateTime? PublishDate { get; set; }
    }

    public class BlogPostUpdateDto
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Content { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Keywords { get; set; }
        public string Status { get; set; } // Draft, Published, Archived
        public bool IsDraft { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
        public int CategoryId { get; set; }
        public int TagId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
