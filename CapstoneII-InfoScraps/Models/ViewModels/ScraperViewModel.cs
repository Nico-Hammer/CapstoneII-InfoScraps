using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.ViewModels
{
    public class ScraperViewModel
    {
        [Required(ErrorMessage = "Website URL is required")]
        [Url(ErrorMessage = "Please enter a valid website URL")]
        public string WebsiteUrl { get; set; }

        [Required(ErrorMessage = "Email regex is required")]
        public string EmailRegex { get; set; }
    }
}
