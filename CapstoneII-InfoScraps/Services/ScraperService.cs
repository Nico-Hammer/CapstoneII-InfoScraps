using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace CapstoneII_InfoScraps.Services
{
    public class ScraperService
    {
        // Main scraping method using Selenium
        public List<ScrapeResult> Scrape(string url)
        {
            var results = new List<ScrapeResult>();
            var emails = new HashSet<string>();        // Store emails and prevent duplicates
            var phoneNumbers = new HashSet<string>();  // Store phone numbers and prevent duplicates

            IWebDriver driver = null;

            try
            {
                // Try Chrome/Edge first
                try
                {
                    var options = new ChromeOptions();
                    options.AddArgument("--headless");
                    options.AddArgument("--disable-gpu");
                    options.AddArgument("--no-sandbox");

                    driver = new ChromeDriver(options);
                }
                catch
                {
                    // If Chrome/Edge fails, use Firefox
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.AddArgument("--headless");

                    driver = new FirefoxDriver(firefoxOptions);
                }

                // Navigate to the requested URL
                driver.Navigate().GoToUrl(url);

                // Wait up to 1 second for page to load
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));

                try
                {
                    wait.Until(d =>
                        ((IJavaScriptExecutor)d)
                        .ExecuteScript("return document.readyState")
                        .Equals("complete")
                    );
                }
                catch
                {
                    // Continue even if timeout occurs
                }

                // Get the full page source
                var pageSource = driver.PageSource;

                // Extract emails from the page using regex
                var emailRegex = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
                var emailMatches = emailRegex.Matches(pageSource);

                foreach (Match match in emailMatches)
                {
                    var cleanEmail = match.Value.ToLower().Trim();
                    emails.Add(cleanEmail);
                }

                // Extract phone numbers from the page using regex
                var phoneRegex = new Regex(@"\+?1?\s*\(?\d{3}\)?[ .-]?\d{3}[ .-]?\d{4}");

                // Only allow certain area codes for Alberta and toll-free numbers
                var allowedAreaCodes = new HashSet<string>
                {
                    "403", "587", "780", "825", // Alberta
                    "800", "888", "877", "866", "855", "844", "833" // Toll-free
                };

                var phoneMatches = phoneRegex.Matches(pageSource);

                foreach (Match match in phoneMatches)
                {
                    // Remove all non-digit characters
                    var digits = Regex.Replace(match.Value, @"[^\d]", "");

                    // Remove leading 1 if it exists
                    if (digits.Length == 11 && digits.StartsWith("1"))
                    {
                        digits = digits.Substring(1);
                    }

                    // Only use numbers that have exactly 10 digits
                    if (digits.Length == 10)
                    {
                        var areaCode = digits.Substring(0, 3);

                        // Only include allowed area codes
                        if (allowedAreaCodes.Contains(areaCode))
                        {
                            var formatted = $"{digits.Substring(0, 3)}-{digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
                            phoneNumbers.Add(formatted);
                        }
                    }
                }

                // Extract names from emails (optional)
                var names = new List<string>();
                foreach (var email in emails)
                {
                    var namePart = email.Split('@')[0];
                    var parts = namePart.Split(new[] { '.', '_' }, StringSplitOptions.RemoveEmptyEntries);

                    var finalName = "";
                    foreach (var part in parts)
                    {
                        if (part.Length > 0)
                            finalName += char.ToUpper(part[0]) + part.Substring(1) + " ";
                    }

                    finalName = finalName.Trim();
                    if (!string.IsNullOrEmpty(finalName))
                        names.Add(finalName);
                }

                // Create a result object with all extracted information
                var result = new ScrapeResult
                {
                    Url = url,
                    Emails = new List<string>(emails),
                    PhoneNumbers = new List<string>(phoneNumbers),
                    Names = names
                };

                results.Add(result);
            }
            catch
            {
                // Ignore errors for now
            }
            finally
            {
                // Close the browser properly
                driver?.Quit();
            }

            return results;
        }
    }

    // Class to store scraped contact information from one website
    public class ScrapeResult
    {
        public string Url { get; set; } = "";
        public List<string> Emails { get; set; } = new List<string>();
        public List<string> PhoneNumbers { get; set; } = new List<string>();
        public List<string> Names { get; set; } = new List<string>();
    }
}