using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.DB;

public class EmailTemplates
{
    // pk id
    [Required]
    public int Id { get; set; }
    // fk accountid
    public int AccountId { get; set; }

    public Accounts Account { get; set; } = null!;
    // template data
    public List<string>? Template_Data {get; set;}
    // template variables
    public List<string>? Template_Variables {get; set;}
}