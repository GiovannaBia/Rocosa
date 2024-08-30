using Microsoft.AspNetCore.Mvc;

namespace Rocosa.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
