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
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }
    }
}
