using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAdminPanel.Models.DTOs
{
    [NotMapped]
    public class CategoryCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
    }
    [NotMapped]
    public class CategoryUpdateDto
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public string UpdatedBy { get; set; }
            public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
    
}
