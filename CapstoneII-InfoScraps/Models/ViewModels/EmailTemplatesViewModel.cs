using CapstoneII_InfoScraps.Models.DB;

namespace CapstoneII_InfoScraps.Models.ViewModels;

public class EmailTemplatesViewModel
{
    public List<ParsedTemplate> DefaultTemplates { get; set; } = new();
    public List<ParsedTemplate> UserTemplates { get; set; } = new();
    public List<ScrapedData> Leads { get; set; } = new();
    public string SenderName { get; set; } = "";
}

public class ParsedTemplate
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
    public List<string> Variables { get; set; } = new();
    public bool IsDefault { get; set; }
}
