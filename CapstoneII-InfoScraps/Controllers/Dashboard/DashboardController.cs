using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return View();
            }
            var email = HttpContext.Session.GetString("Email");
            var phone = HttpContext.Session.GetString("PhoneNumber");
            ViewData["Username"] = username;
            ViewData["Email"] = email;
            ViewData["PhoneNumber"] = phone;
            return View();
        }
    }
}
