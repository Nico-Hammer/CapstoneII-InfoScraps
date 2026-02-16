using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.DB;

public class ScrapedData
{
   // pk id
   public int Id { get; set; }
   // fk accountid
   public int AccountId { get; set; }
   public Account Account { get; set; } = null!;
   // scraped name
   public string? Scraped_Name { get; set; }
   // scraped email
   public string? Scraped_Email { get; set; }
   // scraped phone number
   public string? Scraped_Phone{ get; set; }
   // time of scrape
   [Required]
   public DateTime Date_Of_Scrape { get; set; }
   // website scraped from
   [Required] public string Website { get; set; } = null!;
}