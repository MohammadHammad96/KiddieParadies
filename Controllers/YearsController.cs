using Microsoft.AspNetCore.Mvc;

namespace KiddieParadies.Controllers
{
    public class YearsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}