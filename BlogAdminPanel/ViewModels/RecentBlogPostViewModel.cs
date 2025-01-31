namespace BlogAdminPanel.ViewModels
{
    public class RecentBlogPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public string Status { get; set; }
    }
}
