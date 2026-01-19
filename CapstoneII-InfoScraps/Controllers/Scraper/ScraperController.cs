using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.Scraper
{
    public class ScraperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
