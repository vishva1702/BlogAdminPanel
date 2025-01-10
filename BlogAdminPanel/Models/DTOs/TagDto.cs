namespace BlogAdminPanel.Models.DTOs
{
    public class TagCreateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
    }
    public class TagUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}
