using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Response;
using ProductService_5000.Services;
using Microsoft.AspNetCore.Http;

namespace ProductService_5000.Controllers
{
	[Route("[controller]/[action]")]
	public class CartController : Controller
	{
		private readonly IS_Cart _s_Cart;
		private readonly MRes_InfoUser _currentUser;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const string IdBranchSessionKey = "IdBranch";

		public CartController(IS_Cart cart, CurrentUserHelper currentUser, IHttpContextAccessor httpContextAccessor)
		{
			_s_Cart = cart;
			_currentUser = currentUser.GetCurrentUser();
			_httpContextAccessor = httpContextAccessor;
		}

		private int IdBranch
		{
			get
			{
				return _httpContextAccessor.HttpContext.Session.GetInt32(IdBranchSessionKey) ?? 0;
			}
			set
			{
				_httpContextAccessor.HttpContext.Session.SetInt32(IdBranchSessionKey, value);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var getAll = await _s_Cart.GetAll(IdBranch, _currentUser);
			return View(getAll); 
		}

		[HttpPost]
		public async Task<IActionResult> Add(MRes_Cart request)
		{
			var add = await _s_Cart.Add(request, _currentUser);
			return Json(add);
		}
	}
}
