using System.ComponentModel.DataAnnotations;

namespace BlogAdminPanel.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [StringLength(150, ErrorMessage = "Meta Title cannot exceed 150 characters.")]
        public string MetaTitle { get; set; }

        [StringLength(300, ErrorMessage = "Meta Description cannot exceed 300 characters.")]
        public string MetaDescription { get; set; }

        [StringLength(500, ErrorMessage = "Keywords cannot exceed 500 characters.")]
        public string Keywords { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; }

        public bool IsDraft { get; set; } = false;

        public bool IsPublished { get; set; } = false;

        public bool IsArchived { get; set; } = false;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? PublishedDate { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Created By is required.")]
        [StringLength(100, ErrorMessage = "Created By cannot exceed 100 characters.")]
        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [StringLength(100, ErrorMessage = "Updated By cannot exceed 100 characters.")]
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tag is required.")]
        public int TagId { get; set; }

        [StringLength(255, ErrorMessage = "Image path cannot exceed 255 characters.")]
        public string Image { get; set; }

        public int Views { get; set; } = 0;

        public int Likes { get; set; } = 0;

        public int Shares { get; set; } = 0;

        public Category Category { get; set; }

        public Tag Tag { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
