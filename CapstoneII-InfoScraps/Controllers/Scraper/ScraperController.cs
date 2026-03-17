using CapstoneII_InfoScraps.Models.ViewModels;
using CapstoneII_InfoScraps.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
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
                    var accountID = HttpContext.Session.GetInt32("AccountID");
                    if (accountID == null)
                    {
                        model.ErrorMessage = "User session expired.";
                        return View(model);
                    }

                    // Clear previous model data
                    model.Emails.Clear();
                    model.PhoneNumbers.Clear();
                    model.Names.Clear();

                    // Loop through all results from the scraper
                    foreach (var result in resultList)
                    {
                        // Populate model lists (for showing on the page)
                        if (result.Emails != null)
                            model.Emails.AddRange(result.Emails);
                        if (result.PhoneNumbers != null)
                            model.PhoneNumbers.AddRange(result.PhoneNumbers);
                        if (result.Names != null)
                            model.Names.AddRange(result.Names);

                        // Save each email as a separate contact
                        if (result.Emails != null && result.Emails.Any())
                        {
                            foreach (var email in result.Emails)
                            {
                                var scraped = new ScrapedData
                                {
                                    AccountId = (int)accountID,
                                    Website = model.WebsiteUrl,
                                    Date_Of_Scrape = DateTime.UtcNow,
                                    Scraped_Email = email,
                                    Scraped_Phone = result.PhoneNumbers?.FirstOrDefault(),
                                    Scraped_Name = result.Names?.FirstOrDefault() ?? "No name found"
                                };

                                _context.ScrapedData.Add(scraped);
                            }
                        }
                        else if (result.PhoneNumbers != null && result.PhoneNumbers.Any())
                        {
                            // Fallback: save phone numbers if no emails were found
                            foreach (var phone in result.PhoneNumbers)
                            {
                                var scraped = new ScrapedData
                                {
                                    AccountId = (int)accountID,
                                    Website = model.WebsiteUrl,
                                    Date_Of_Scrape = DateTime.UtcNow,
                                    Scraped_Email = null,
                                    Scraped_Phone = phone,
                                    Scraped_Name = result.Names?.FirstOrDefault() ?? "No name found"
                                };

                                _context.ScrapedData.Add(scraped);
                            }
                        }
                    }

                    // Save all scraped contacts to the database
                    _context.SaveChanges();

                    // Set success message if scraping found results
                    if (model.Emails.Any() || model.PhoneNumbers.Any())
                    {
                        model.SuccessMessage = $"Scraping completed for {model.WebsiteUrl}.";
                    }
                    else
                    {
                        model.ErrorMessage = "No emails or phone numbers were found on this website.";
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
        
        [HttpPost]
        public IActionResult Edit(int id, string name, string email, string phone)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var scrapedData = _context.ScrapedData
                .FirstOrDefault(sd => sd.Id == id && sd.AccountId == accountID.Value);

            if (scrapedData == null)
                return RedirectToAction("Index");

            scrapedData.Scraped_Name = name;
            scrapedData.Scraped_Email = email;
            scrapedData.Scraped_Phone = phone;
            _context.SaveChanges();

            TempData["Success"] = "Scraped data updated successfully.";
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var scrapedData = _context.ScrapedData
                .FirstOrDefault(sd => sd.Id == id && sd.AccountId == accountID.Value);

            if (scrapedData != null)
            {
                _context.ScrapedData.Remove(scrapedData);
                _context.SaveChanges();
                TempData["Success"] = "Scraped data deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}