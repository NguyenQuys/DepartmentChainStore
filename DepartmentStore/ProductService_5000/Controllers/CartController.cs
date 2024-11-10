using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Response;
using ProductService_5000.Services;
using Microsoft.AspNetCore.Http;

namespace ProductService_5000.Controllers
{
	[Route("Cart/[action]")]
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
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var getAll = await _s_Cart.GetAll(IdBranch, _currentUser);
			return Json(getAll); 
		}

		[HttpPost]
		public async Task<IActionResult> Add(MRes_Cart request)
		{
			var add = await _s_Cart.Add(request, _currentUser);
			return Json(add);
		}

		[HttpDelete]
		public async Task<IActionResult> Delete(int idProduct)
		{
			var delete = await _s_Cart.Delete(idProduct, _currentUser);
			return Json(delete);
		}

		//[HttpPost]
		//public async Task<vcv>

		// Invoice
		[HttpGet]
		public async Task<IActionResult> InvoiceIndex(List<MRes_Product> listRequest)
		{
			return View(listRequest);
		}
	}
}
