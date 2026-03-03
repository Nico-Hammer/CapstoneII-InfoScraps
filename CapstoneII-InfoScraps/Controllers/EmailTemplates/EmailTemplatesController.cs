using CapstoneII_InfoScraps.Models.DB;
using CapstoneII_InfoScraps.Models.ViewModels;
using CapstoneII_InfoScraps.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.EmailTemplates
{
    public class EmailTemplatesController : Controller
    {
        private readonly AppDbContext _context;

        public EmailTemplatesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
            {
                var acct = _context.Accounts.FirstOrDefault(a => a.User.Id == userID.Value);
                if (acct == null)
                    return RedirectToAction("Index", "Login");
                accountID = acct.Id;
                HttpContext.Session.SetInt32("AccountID", accountID.Value);
            }

            var userTemplates = _context.EmailTemplates
                .Where(e => e.AccountId == accountID.Value)
                .ToList()
                .Select(t => new ParsedTemplate
                {
                    Id = t.Id,
                    Name = t.Template_Data?.ElementAtOrDefault(0) ?? "Untitled",
                    Subject = t.Template_Data?.ElementAtOrDefault(1) ?? "",
                    Body = t.Template_Data?.ElementAtOrDefault(2) ?? "",
                    Variables = t.Template_Variables ?? new List<string>(),
                    IsDefault = false
                })
                .ToList();

            var leads = _context.ScrapedData
                .Where(s => s.AccountId == accountID.Value)
                .OrderByDescending(s => s.Date_Of_Scrape)
                .ToList();

            var user = _context.Users.FirstOrDefault(u => u.Id == userID.Value);

            var viewModel = new EmailTemplatesViewModel
            {
                DefaultTemplates = DefaultEmailTemplateService.GetDefaultTemplates(),
                UserTemplates = userTemplates,
                Leads = leads,
                SenderName = user?.Username ?? ""
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(string name, string subject, string body)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var template = new EmailTemplate
            {
                AccountId = accountID.Value,
                Template_Data = new List<string> { name, subject, body },
                Template_Variables = DefaultEmailTemplateService.ExtractVariables(subject + " " + body)
            };

            _context.EmailTemplates.Add(template);
            _context.SaveChanges();

            TempData["Success"] = "Template created successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, string name, string subject, string body)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var template = _context.EmailTemplates
                .FirstOrDefault(t => t.Id == id && t.AccountId == accountID.Value);

            if (template == null)
                return RedirectToAction("Index");

            template.Template_Data = new List<string> { name, subject, body };
            template.Template_Variables = DefaultEmailTemplateService.ExtractVariables(subject + " " + body);

            _context.SaveChanges();

            TempData["Success"] = "Template updated successfully.";
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

            var template = _context.EmailTemplates
                .FirstOrDefault(t => t.Id == id && t.AccountId == accountID.Value);

            if (template != null)
            {
                _context.EmailTemplates.Remove(template);
                _context.SaveChanges();
                TempData["Success"] = "Template deleted successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}
