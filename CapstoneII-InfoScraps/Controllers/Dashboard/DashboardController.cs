using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CapstoneII_InfoScraps.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
            {
                return View();
            }

            var account = _context.Accounts
                .Where(a => a.User.Id == userID.Value)
                .Include(a => a.User)
                .Include(a => a.Email_Templates)
                .Include(a => a.Scraped_Data)
                .ToList();

            if (account.Any())
            {
                HttpContext.Session.SetInt32("AccountID",account.First().Id);
            }
            return View(account);
        }

        [HttpGet]
        public IActionResult ExportCsv()
        {
            var accountId = HttpContext.Session.GetInt32("AccountID");
            if (accountId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var scrapedData = _context.ScrapedData
                .Where(s => s.AccountId == accountId.Value)
                .OrderByDescending(s => s.Date_Of_Scrape)
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Date,Email,Name,Phone,Website");

            foreach (var record in scrapedData)
            {
                var date = EscapeCsvField(record.Date_Of_Scrape.ToString("yyyy-MM-dd HH:mm:ss"));
                var email = EscapeCsvField(record.Scraped_Email ?? "");
                var name = EscapeCsvField(record.Scraped_Name ?? "");
                var phone = EscapeCsvField(record.Scraped_Phone ?? "");
                var website = EscapeCsvField(record.Website);
                csv.AppendLine($"{date},{email},{name},{phone},{website}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"infoscraps_leads_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

        private static string EscapeCsvField(string field)
        {
            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }

        [HttpPost]
        public IActionResult Edit(int id, string name, string email, string phone)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var scrapedData = _context.ScrapedData
                .FirstOrDefault(sd => sd.Id == id && sd.AccountId == accountID.Value);

            if (scrapedData == null)
                return RedirectToAction("Index");

            scrapedData.Scraped_Name = name;
            scrapedData.Scraped_Email = email;
            scrapedData.Scraped_Phone = phone;
            _context.SaveChanges();

            TempData["Success"] = "Lead updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var scrapedData = _context.ScrapedData
                .FirstOrDefault(sd => sd.Id == id && sd.AccountId == accountID.Value);

            if (scrapedData != null)
            {
                _context.ScrapedData.Remove(scrapedData);
                _context.SaveChanges();
                TempData["Success"] = "Lead deleted successfully.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
