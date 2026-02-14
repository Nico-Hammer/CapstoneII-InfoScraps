using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.DB;

public class Account
{
    // pk id
    [Required]
    public int Id { get; set; }
    // fk user
    public Users User { get; set; }
    // fk email templates
    public ICollection<EmailTemplate> Email_Templates { get; set; }
    // fk scraped data
    public ICollection<ScrapedData> Scraped_Data { get; set; }
}