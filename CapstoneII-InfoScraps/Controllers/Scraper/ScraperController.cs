using CapstoneII_InfoScraps.Models.ViewModels;
using CapstoneII_InfoScraps.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Linq;
using CapstoneII_InfoScraps.Models.DB;

namespace CapstoneII_InfoScraps.Controllers.Scraper
{
    public class ScraperController : Controller
    {
        private readonly ScraperService _scraperService;
        private readonly AppDbContext _context;

        // Regex pattern to validate website URLs
        private const string URLRegexPattern =
            @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+?$";

        public ScraperController(ScraperService scraperService, AppDbContext context)
        {
            // Inject the scraper service
            _scraperService = scraperService;
            _context = context;
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

                    // Populate model lists
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

                        var scraped = new ScrapedData();
                        var accountID = HttpContext.Session.GetInt32("AccountID");
                        if (accountID == null)
                        {
                            model.ErrorMessage = "User session expired.";
                            return View(model);
                        }

                        // Save each email as a separate contact
                        foreach (var email in model.Emails)
                        {
                            var scraped = new ScrapedData
                            {
                                AccountId = (int)accountID,
                                Website = model.WebsiteUrl,
                                Date_Of_Scrape = DateTime.UtcNow,
                                Scraped_Email = email,
                                Scraped_Phone = model.PhoneNumbers.FirstOrDefault(),
                                Scraped_Name = model.Names.FirstOrDefault() ?? "No name found"
                            };

                            _context.ScrapedData.Add(scraped);
                        }

                        // If no emails were found, fall back to saving phone numbers
                        if (!model.Emails.Any())
                        {
                            foreach (var phone in model.PhoneNumbers)
                            {
                                var scraped = new ScrapedData
                                {
                                    AccountId = (int)accountID,
                                    Website = model.WebsiteUrl,
                                    Date_Of_Scrape = DateTime.UtcNow,
                                    Scraped_Email = null,
                                    Scraped_Phone = phone,
                                    Scraped_Name = model.Names.FirstOrDefault() ?? "No name found"
                                };

                                _context.ScrapedData.Add(scraped);
                            }
                        }

                        _context.ScrapedData.Add(scraped);
                        _context.SaveChanges();
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