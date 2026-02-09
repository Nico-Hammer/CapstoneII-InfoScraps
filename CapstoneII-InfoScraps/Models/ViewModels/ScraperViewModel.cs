using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.ViewModels
{
    public class ScraperViewModel
    {
        [Required(ErrorMessage = "Website URL is required")]
        [Url(ErrorMessage = "Please enter a valid website URL")]
        public string WebsiteUrl { get; set; }

        // Scraped data
        public List<string> Names { get; set; } = new();
        public List<string> Emails { get; set; } = new();

        // Feedback to the user
        public string ErrorMessage { get; set; }
    }
}
