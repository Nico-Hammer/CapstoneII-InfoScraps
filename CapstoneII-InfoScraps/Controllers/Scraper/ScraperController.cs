using Microsoft.AspNetCore.Mvc;
using CapstoneII_InfoScraps.Models.ViewModels;
using System.Text.RegularExpressions;

namespace CapstoneII_InfoScraps.Controllers.Scraper
{
    public class ScraperController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new ScraperViewModel
            {
                EmailRegex = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}"
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(ScraperViewModel model)
        {
            // Check required fields and URL format
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Make sure the regex is valid
            try
            {
                _ = new Regex(model.EmailRegex);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError(
                    nameof(model.EmailRegex),
                    "Email regex is not valid."
                    );

                return View(model);
            }

            return View(model);
        }
    }
}
