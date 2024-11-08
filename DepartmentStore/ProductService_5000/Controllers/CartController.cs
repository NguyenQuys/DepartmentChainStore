using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Request;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    [Route("[controller]/[action]")]
    public class CartController : Controller
    {
        private readonly IS_Cart _s_Cart;
        private readonly MRes_InfoUser _currentUser;

        public CartController(IS_Cart cart, CurrentUserHelper currentUser)
        {
            _s_Cart = cart;
            _currentUser = currentUser.GetCurrentUser();
        }

        public async Task<IActionResult> GetAll()
        {
            var getAll = await _s_Cart.GetAll(_currentUser);
            return View(); // add view later
        }

        public async Task<IActionResult> Add(MRes_Cart request)
        {
            var add = await _s_Cart.Add(request,_currentUser);
            return Json(add);
        }
    }
}
