using System.Text.RegularExpressions;
using CapstoneII_InfoScraps.Models.ViewModels;

namespace CapstoneII_InfoScraps.Services;

/// <summary>
/// Provides the built-in default email templates available to all users.
/// </summary>
public static class DefaultEmailTemplateService
{
    public static List<ParsedTemplate> GetDefaultTemplates()
    {
        var templates = new List<ParsedTemplate>
        {
            new()
            {
                Name = "The Quick Check",
                Subject = "Quick question, {{business_name}}",
                Body =
                    "Hi {{first_name}},\n\n" +
                    "Quick question. Who usually handles {{specific_problem}} at {{business_name}}?\n\n" +
                    "I noticed {{personalized_observation}} and thought I would reach out.\n\n" +
                    "If this is not you, no worries at all.\n\n" +
                    "Thanks,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Gentle Idea",
                Subject = "One idea for {{business_name}}",
                Body =
                    "Hi {{first_name}},\n" +
                    "I came across {{business_name}} while looking at {{industry_or_location}} businesses and noticed {{specific_problem}}.\n" +
                    "We have seen a simple fix help with this before, so I thought it might be worth sharing.\n" +
                    "Happy to explain if you are interested.\n" +
                    "Best,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Pattern Spotter",
                Subject = "Seeing this across {{industry}} businesses",
                Body =
                    "Hi {{first_name}},\n" +
                    "I have been looking at a number of {{industry}} businesses recently and noticed a common pattern around {{specific_problem}}.\n" +
                    "When I checked {{business_name}}, it looked like there could be an opportunity there too.\n" +
                    "Let me know if you would like more context.\n" +
                    "Regards,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Social Proof Lite",
                Subject = "Something that worked for a similar business",
                Body =
                    "Hi {{first_name}},\n" +
                    "We recently helped a business similar to {{business_name}} and saw {{specific_result}}.\n" +
                    "When I looked at your setup, it reminded me of where they started.\n" +
                    "If you want, I can share what made the difference.\n" +
                    "Best,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Ultra Short Ask",
                Subject = "Open to a quick idea",
                Body =
                    "Hi {{first_name}},\n" +
                    "Would you be open to a quick idea that has helped other {{industry}} businesses?\n" +
                    "Totally fine if not.\n" +
                    "Thanks,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Direct Owner Note",
                Subject = "Question about {{business_name}}",
                Body =
                    "Hi {{first_name}},\n" +
                    "I will keep this short.\n" +
                    "I work with {{industry}} businesses on {{specific_outcome}} and noticed something interesting at {{business_name}}.\n" +
                    "If it is relevant, I am happy to explain.\n" +
                    "Best,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Curious Observation",
                Subject = "Noticed this at {{business_name}}",
                Body =
                    "Hi {{first_name}},\n" +
                    "While looking at {{business_name}}, I noticed {{personalized_observation}}.\n" +
                    "It caught my attention because it often connects to {{specific_problem}} for similar businesses.\n" +
                    "Let me know if you want me to elaborate.\n" +
                    "Regards,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Low Commitment Share",
                Subject = "Happy to share an idea",
                Body =
                    "Hi {{first_name}},\n" +
                    "I had an idea that could help with {{specific_problem}} at {{business_name}}.\n" +
                    "No pitch attached. Just something we have seen work well before.\n" +
                    "If you want to hear it, let me know.\n" +
                    "Thanks,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Timing Check",
                Subject = "Is now a bad time",
                Body =
                    "Hi {{first_name}},\n" +
                    "Just wanted to check if now is a bad time to share an idea related to {{specific_problem}} at {{business_name}}.\n" +
                    "If it is, I am happy to circle back later.\n" +
                    "Best,\n" +
                    "{{sender_name}}",
                IsDefault = true
            },
            new()
            {
                Name = "The Straightforward Intro",
                Subject = "Quick intro",
                Body =
                    "Hi {{first_name}},\n" +
                    "Quick introduction. I work with {{industry}} businesses and focus on {{specific_outcome}}.\n" +
                    "I came across {{business_name}} and thought there could be a relevant opportunity.\n" +
                    "Open to a short conversation if it makes sense.\n" +
                    "Regards,\n" +
                    "{{sender_name}}",
                IsDefault = true
            }
        };

        // Auto-populate variables for each template
        foreach (var template in templates)
        {
            template.Variables = ExtractVariables(template.Subject + " " + template.Body);
        }

        return templates;
    }

    /// <summary>
    /// Extracts all {{variable_name}} placeholders from text.
    /// </summary>
    public static List<string> ExtractVariables(string text)
    {
        var matches = Regex.Matches(text, @"\{\{(\w+)\}\}");
        return matches.Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();
    }
}
