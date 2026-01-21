using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
