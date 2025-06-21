using Microsoft.AspNetCore.Mvc;

namespace DYAT.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Index2()
        {
            return View();
        }
        
        public IActionResult Index3()
        {
            return View();
        }
        
        public IActionResult About()
        {
            return View();
        }
    }
}