using Microsoft.AspNetCore.Mvc;

namespace BlogAdminPanel.Controllers
{
    public class EditorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
