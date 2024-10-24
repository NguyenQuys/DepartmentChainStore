using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService_5002.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly MRes_InfoUser _currentUser;

        public HomeController(CurrentUserHelper currentUser)
        {
            _currentUser = currentUser.GetCurrentUser();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index() 
        { 
            return View(_currentUser); 
        }
    }
}
