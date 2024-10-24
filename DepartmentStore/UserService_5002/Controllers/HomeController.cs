using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService_5002.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        public HomeController() { }

        [HttpGet]
        [Authorize]
        public IActionResult Index() 
        { 
            return View(); 
        }
    }
}
