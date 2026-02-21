using CapstoneII_InfoScraps.Models.DB;
using CapstoneII_InfoScraps.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.Account
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index",model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                ViewData["ErrorMessage"] = "Invalid username or password";
                return View("Index",model);
            }
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (!result.Equals(PasswordVerificationResult.Success))
            {
                ViewData["ErrorMessage"] = "Invalid username or password";
                return View("Index",model);
            }
            HttpContext.Session.SetInt32("UserID",user.Id);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}
