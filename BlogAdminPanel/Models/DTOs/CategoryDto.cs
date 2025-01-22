using System.ComponentModel.DataAnnotations.Schema;

namespace BlogAdminPanel.Models.DTOs
{
    public class CategoryCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CategoryUpdateDto
    {
            public int Id { get; set; }
            public string Name { get; set; }
            
    }
    
}
