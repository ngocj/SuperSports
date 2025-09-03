using Microsoft.AspNetCore.Mvc;

namespace SP.WebApp.Controllers
{
    public class PageController : Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
