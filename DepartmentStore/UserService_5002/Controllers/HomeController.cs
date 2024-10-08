using Microsoft.AspNetCore.Mvc;

namespace UserService_5002.Controllers
{
    public class HomeController : Controller
    {
        public HomeController() { }

        [HttpGet]
        public IActionResult Index() 
        { 
            return View(); 
        }
    }
}
