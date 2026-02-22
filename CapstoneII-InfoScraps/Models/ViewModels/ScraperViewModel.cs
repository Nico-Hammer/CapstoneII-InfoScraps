using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CapstoneII_InfoScraps.Models.ViewModels
{
    public class ScraperViewModel
    {
        // User input for the website they want to scrape
        [Required(ErrorMessage = "Website URL is required")]
        [Url(ErrorMessage = "Please enter a valid website URL")]
        public string WebsiteUrl { get; set; } = string.Empty;

        // Lists to store the scraped results
        public List<string> Names { get; set; } = new List<string>();
        public List<string> Emails { get; set; } = new List<string>();
        public List<string> PhoneNumbers { get; set; } = new List<string>();

        // Messages shown back to the user
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
