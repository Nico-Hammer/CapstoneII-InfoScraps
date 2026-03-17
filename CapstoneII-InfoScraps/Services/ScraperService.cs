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

                // Wait up to 1 second for page to load
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1));

                // Store the base website URL
                var baseUri = new Uri(url);

                // Queue used to keep track of pages to visit
                var pagesToVisit = new Queue<string>();

                // Track pages already visited to avoid loops
                var visitedPages = new HashSet<string>();

                // Start crawling from the main page
                pagesToVisit.Enqueue(url);

                // Limit number of pages to crawl
                int maxPages = 25;

                // Ignore known system email domains
                var blockedDomains = new HashSet<string>
                {
                    "sentry.io",
                    "sentry.wixpress.com",
                    "sentry-next.wixpress.com"
                };

                // Crawl pages until queue empty or limit reached
                while (pagesToVisit.Count > 0 && visitedPages.Count < maxPages)
                {
                    var currentPage = pagesToVisit.Dequeue();

                    // Skip pages already visited
                    if (visitedPages.Contains(currentPage))
                        continue;

                    visitedPages.Add(currentPage);

                    // Navigate to page
                    driver.Navigate().GoToUrl(currentPage);

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

                        // Split email to check domain
                        var parts = cleanEmail.Split('@');
                        if (parts.Length != 2)
                            continue;

                        var domain = parts[1];

                        // Skip blocked system domains
                        if (blockedDomains.Contains(domain))
                            continue;

                        // Skip very long emails that are likely hashes
                        if (cleanEmail.Length > 40)
                            continue;

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

                    // Get all links on the page
                    var links = driver.FindElements(By.TagName("a"));

                    foreach (var link in links)
                    {
                        var href = link.GetAttribute("href");

                        if (string.IsNullOrWhiteSpace(href))
                            continue;

                        Uri uri;

                        // Convert relative links to full URLs
                        if (!Uri.TryCreate(href, UriKind.Absolute, out uri))
                        {
                            uri = new Uri(baseUri, href);
                        }

                        // Stay inside the same website
                        if (uri.Host != baseUri.Host)
                            continue;

                        var linkUrl = uri.ToString();

                        // Add new page to crawl queue
                        if (!visitedPages.Contains(linkUrl))
                        {
                            pagesToVisit.Enqueue(linkUrl);
                        }
                    }
                }

                // Extract names from page meta, title, H1, fallback to emails
                var names = new List<string>();
                try
                {
                    // Check meta tag for Open Graph site name
                    foreach (var meta in driver.FindElements(By.XPath("//meta[@property='og:site_name']")))
                    {
                        var content = meta.GetAttribute("content");
                        if (!string.IsNullOrWhiteSpace(content))
                            names.Add(content.Trim());
                    }

                    // Check meta tag for author
                    foreach (var meta in driver.FindElements(By.XPath("//meta[@name='author']")))
                    {
                        var content = meta.GetAttribute("content");
                        if (!string.IsNullOrWhiteSpace(content))
                            names.Add(content.Trim());
                    }

                    // Use the page title
                    var title = driver.Title;
                    if (!string.IsNullOrWhiteSpace(title))
                        names.Add(title.Split('|')[0].Split('-')[0].Trim());

                    // Use the first H1 if available
                    var h1 = driver.FindElements(By.TagName("h1"));
                    if (h1.Count > 0 && !string.IsNullOrWhiteSpace(h1[0].Text))
                        names.Add(h1[0].Text.Trim());
                }
                catch
                {
                    // Ignore small errors in page name extraction
                }

                // Fallback to email-based names if nothing found
                if (names.Count == 0)
                {
                    foreach (var email in emails)
                    {
                        var namePart = email.Split('@')[0];
                        var parts = namePart.Split(new[] { '.', '_' }, StringSplitOptions.RemoveEmptyEntries);
                        var finalName = "";
                        foreach (var part in parts)
                        {
                            if (!string.IsNullOrEmpty(part))
                                finalName += char.ToUpper(part[0]) + part.Substring(1) + " ";
                        }
                        finalName = finalName.Trim();
                        if (!string.IsNullOrEmpty(finalName))
                            names.Add(finalName);
                    }
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