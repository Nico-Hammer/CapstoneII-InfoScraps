using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.DB;

public class Account
{
    // pk id
    public int Id { get; set; }
    // fk user
    public User User { get; set; } = null!;
    // fk email templates
    public ICollection<EmailTemplate> Email_Templates { get; set; } = new List<EmailTemplate>();
    // fk scraped data
    public ICollection<ScrapedData> Scraped_Data { get; set; } = new List<ScrapedData>();
}