using Microsoft.AspNetCore.Mvc;

namespace CapstoneII_InfoScraps.Controllers.EmailTemplates
{
    public class EmailTemplatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
