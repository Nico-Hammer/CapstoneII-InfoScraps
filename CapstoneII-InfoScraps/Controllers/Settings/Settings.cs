using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapstoneII_InfoScraps.Controllers.Settings
{
    public class Settings : Controller
    {
        private readonly AppDbContext _context;

        public Settings(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var account = _context.Accounts
                .Where(a => a.User.Id == userID.Value)
                .Include(a => a.User)
                .Include(a => a.Email_Templates)
                .Include(a => a.Scraped_Data)
                .FirstOrDefault();

            return View(account);
        }
    }
}
