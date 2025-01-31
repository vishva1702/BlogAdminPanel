namespace BlogAdminPanel.ViewModels
{
    public class DashboardViewModel
    {
        
            public int TotalBlogPosts { get; set; }
            public int TotalUsers { get; set; }
            public int TotalTags { get; set; }
            public int TotalCategories { get; set; }
            public int TotalComments { get; set; }

            // Data for the line chart
            public List<int> LikesData { get; set; }
            public List<int> SharesData { get; set; }
            public List<int> ViewsData { get; set; }

            public List<RecentBlogPostViewModel> RecentBlogPosts { get; set; }
        }

    }

