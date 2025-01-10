using Microsoft.AspNetCore.Mvc;

namespace BlogAdminPanel.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
