using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.DB;

public class Accounts
{
    // pk id
    [Required]
    public int Id { get; set; }
    // fk user
    public ICollection<Users> Users { get; set; }
    // fk email templates
    public ICollection<EmailTemplates> Email_Templates { get; set; }
    // fk scraped data
    public ICollection<ScrapedData> Scraped_Data { get; set; }
}