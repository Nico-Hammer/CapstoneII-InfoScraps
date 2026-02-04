using CapstoneII_InfoScraps.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.Scraper
{
    public class ScraperController : Controller
    {
        private const string URLRegexPattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+?$";
        
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ScraperViewModel());
        }

        [HttpPost]
        public IActionResult Index(ScraperViewModel model)
        {
            // Check required fields and URL format
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(!System.Text.RegularExpressions.Regex.IsMatch(model.WebsiteUrl, URLRegexPattern))
            {
                ModelState.AddModelError(nameof(model.WebsiteUrl),"Website URL format is not supported");

                return View(model);
            }


            return View(model);
        }
    }
}
