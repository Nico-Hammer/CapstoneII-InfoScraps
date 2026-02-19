using CapstoneII_InfoScraps.Models.ViewModels;
using CapstoneII_InfoScraps.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Linq;

namespace CapstoneII_InfoScraps.Controllers.Scraper
{
    public class ScraperController : Controller
    {
        private readonly ScraperService _scraperService;

        // Regex pattern to validate website URLs
        private const string URLRegexPattern =
            @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+?$";

        public ScraperController(ScraperService scraperService)
        {
            // Inject the scraper service
            _scraperService = scraperService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Return an empty view model when page first loads
            return View(new ScraperViewModel());
        }

        [HttpPost]
        public IActionResult Index(ScraperViewModel model)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
                return View(model);

            // Validate the website URL format
            if (!Regex.IsMatch(model.WebsiteUrl, URLRegexPattern))
            {
                ModelState.AddModelError(nameof(model.WebsiteUrl),
                    "Website URL format is not supported");
                return View(model);
            }

            try
            {
                // Call the Selenium scraper service
                var resultList = _scraperService.Scrape(model.WebsiteUrl);

                if (resultList.Any())
                {
                    var result = resultList.First();

                    // Populate emails and phone numbers
                    model.Emails = result.Emails ?? new List<string>();
                    model.PhoneNumbers = result.PhoneNumbers ?? new List<string>();

                    // Populate names (optional)
                    model.Names = result.Names ?? new List<string>();

                    // Set error message if nothing was found
                    if (!model.Emails.Any() && !model.PhoneNumbers.Any())
                    {
                        model.ErrorMessage = "No emails or phone numbers were found on this website.";
                    }
                    else
                    {
                        // Set success message if scraping found results
                        model.SuccessMessage = $"Scraping completed for {model.WebsiteUrl}.";
                    }
                }
                else
                {
                    model.ErrorMessage = "No data was returned from the scraper.";
                }
            }
            catch
            {
                // Error message if scraping fails
                model.ErrorMessage = "An error occurred while scraping the website.";
            }

            return View(model);
        }
    }
}
