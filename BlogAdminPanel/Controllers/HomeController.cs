using BlogAdminPanel.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlogAdminPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalPosts = _context.BlogPosts.Count(bp => !bp.IsDeleted);
            ViewBag.PublishedPosts = _context.BlogPosts.Count(bp => bp.Status == "Published");
            ViewBag.DraftPosts = _context.BlogPosts.Count(bp => bp.Status == "Draft");

            ViewBag.TotalCategories = _context.Categories.Count(c => !c.IsDeleted);
            ViewBag.TotalTags = _context.Tags.Count(t => !t.IsDeleted);

            ViewBag.TotalUsers = _context.Users.Count(u => !u.IsDeleted);
            ViewBag.ActiveUsers = _context.Users.Count(u => u.IsActive && !u.IsDeleted);

            ViewBag.PendingComments = _context.Comments.Count(c => !c.IsApproved && !c.IsDeleted);
            ViewBag.ApprovedComments = _context.Comments.Count(c => c.IsApproved && !c.IsDeleted);

            ViewBag.TotalLikes = _context.BlogPosts.Sum(bp => bp.Likes);
            ViewBag.TotalViews = _context.BlogPosts.Sum(bp => bp.Views);
            ViewBag.TotalShares = _context.BlogPosts.Sum(bp => bp.Shares);

            var postEngagement = _context.BlogPosts
                .Where(bp => !bp.IsDeleted)
                .Select(bp => new
                {
                    bp.Title,
                    bp.Views,
                    bp.Likes,
                    bp.Shares
                })
                .ToList();

            ViewBag.PostEngagement = postEngagement;

            return View();
        }
    }
}
