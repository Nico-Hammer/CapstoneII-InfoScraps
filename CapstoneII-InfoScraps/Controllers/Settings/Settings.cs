using CapstoneII_InfoScraps.Models.DB;
using Microsoft.AspNetCore.Identity;
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

        [HttpPost]
        public IActionResult Edit(int id, string username, string email, string phone, string password, string confirmpassword, bool changepass)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (userID == null)
                return RedirectToAction("Index", "Login");

            var accountID = HttpContext.Session.GetInt32("AccountID");
            if (accountID == null)
                return RedirectToAction("Index", "Login");

            var userData = _context.Users
                .FirstOrDefault(u => u.Id == id && u.AccountId == accountID.Value);
            if (userData == null)
                return RedirectToAction("Index");

            userData.Username = username;
            userData.Email = email;
            userData.Phone_Number = phone;
            if (changepass)
            {
                if (password == confirmpassword)
                {
                    var hasher = new PasswordHasher<User>();
                    userData.Password = hasher.HashPassword(userData, confirmpassword);
                }
                else
                {
                    TempData["Error"] = "passwords must match";
                }
            }
            _context.SaveChanges();

            TempData["Success"] = "user data updated successfully.";
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

            var account = _context.Accounts
                .FirstOrDefault(a => a.Id == id && a.User.Id == userID.Value);
            
            if (account != null)
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
                TempData["Success"] = "Account deleted successfully.";
            }

            return RedirectToAction("Index","Login");
        }
    }
}
