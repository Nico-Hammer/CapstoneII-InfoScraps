using CapstoneII_InfoScraps.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace CapstoneII_InfoScraps.Controllers.Scraper
{
    public class ScraperController : Controller
    {
        private const string URLRegexPattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+?$";

        // URL of the Python based scraper API
        private const string ScraperAPIURL = "https://localhost:"; // Fill the rest in once we know what the endpoint will be

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ScraperViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ScraperViewModel model)
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

            // Create an HTTP client to talk to the Python scraper API
            using var client = new HttpClient();

            try
            {
                // Send the validated URL to the scraper API
                var response = await client.PostAsJsonAsync(ScraperAPIURL, new { url = model.WebsiteUrl });

                // Fail if the API returns an error status
                response.EnsureSuccessStatusCode();

                // Read scraped data returned by the API
                var results = await response.Content.ReadFromJsonAsync<ScraperViewModel>();

                // Safely copy scraped results into the view model
                model.Emails = results?.Emails ?? new List<string>();
                model.Names = results?.Names ?? new List<string>();

                // Show a message if nothing was found
                if (!model.Emails.Any() && !model.Names.Any())
                {
                    model.ErrorMessage = "No contact information was found on this website.";
                }
            }
            catch
            {
                // Handle API or network failures
                model.ErrorMessage = "Unable to connect to the scraping service. Please try again later.";
            }

            return View(model);
        }
    }
}
