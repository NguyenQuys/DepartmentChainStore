using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Response;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    [Route("[controller]/[action]")]
    public class CartController : Controller
    {
        private readonly IS_Cart _s_Cart;
        private readonly MRes_InfoUser _currentUser;
        int _idBranch = ProductService_5000.Controllers.ProductController._idBranch;

        public CartController(IS_Cart cart, CurrentUserHelper currentUser)
        {
            _s_Cart = cart;
            _currentUser = currentUser.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var getAll = await _s_Cart.GetAll(_idBranch,_currentUser);
            return View(getAll); // add view later
        }

        [HttpPost]
        public async Task<IActionResult> Add(MRes_Cart request)
        {
            var add = await _s_Cart.Add(request,_currentUser);
            return Json(add);
        }
    }
}
