using BlogAdminPanel.Data;
using BlogAdminPanel.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlogAdminPanel.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Fetch total counts for dashboard summary
            var totalBlogPosts = _context.BlogPosts.Count();
            var totalUsers = _context.Users.Count();
            var totalTags = _context.Tags.Count();
            var totalCategories = _context.Categories.Count();  // Fetch total categories
            var totalComments = _context.Comments.Count();

            // Aggregate data for the line chart
            var likesData = _context.BlogPosts
                .OrderByDescending(bp => bp.CreatedDate)
                .Take(7)
                .Select(bp => bp.Likes)
                .ToList();

            var sharesData = _context.BlogPosts
                .OrderByDescending(bp => bp.CreatedDate)
                .Take(7)
                .Select(bp => bp.Shares)
                .ToList();

            var viewsData = _context.BlogPosts
                .OrderByDescending(bp => bp.CreatedDate)
                .Take(7)
                .Select(bp => bp.Views)
                .ToList();

            // Fetch recent blog posts
            var recentBlogPosts = _context.BlogPosts
                .OrderByDescending(bp => bp.CreatedDate)
                .Take(5) // Fetch the 5 most recent posts
                .Select(bp => new RecentBlogPostViewModel
                {
                    Id = bp.Id,
                    Title = bp.Title,
                    CreatedDate = bp.CreatedDate,
                    Views = bp.Views,
                    Likes = bp.Likes,
                    Status = bp.Status
                })
                .ToList();

            // Pass data to the ViewModel
            var dashboardViewModel = new DashboardViewModel
            {
                TotalBlogPosts = totalBlogPosts,
                TotalUsers = totalUsers,
                TotalTags = totalTags,
                TotalCategories = totalCategories, // Include total categories
                TotalComments = totalComments,
                LikesData = likesData,
                SharesData = sharesData,
                ViewsData = viewsData,
                RecentBlogPosts = recentBlogPosts // Pass recent blog posts to the ViewModel
            };

            return View(dashboardViewModel);
        }

        public IActionResult Error()
        {
            return View();
        }

        [Route("/StatusCodeError/{statuscode}")]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                ViewBag.ErrorMessage = "404 Page is not found";
            }
            else if (statuscode == 500)
            {
                ViewBag.ErrorMessage = "500 Internal Server Error. Something went wrong.";
            }
            return View();
        }
    }
}
