using CapstoneII_InfoScraps.Models.DB;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.Account
{
    public class RegistrationController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationController(AppDbContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var account = new Models.DB.Account();
                account.Email_Templates = new List<EmailTemplate>();
                account.Scraped_Data = new List<ScrapedData>();
                account.User = user;
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine(string.Join(", ", errors));
            return View("Index",user);
        }
    }
}
