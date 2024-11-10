using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Response;
using ProductService_5000.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ProductService_5000.Controllers
{
	[Route("Cart/[action]")]
	public class CartController : Controller
	{
		private readonly IS_Cart _s_Cart;
		private readonly MRes_InfoUser _currentUser;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const string IdBranchSessionKey = "IdBranch";
		private const string LocationBranchSessionKey = "LocationBranch";

		public CartController(IS_Cart cart, CurrentUserHelper currentUser, IHttpContextAccessor httpContextAccessor)
		{
			_s_Cart = cart;
			_currentUser = currentUser.GetCurrentUser();
			_httpContextAccessor = httpContextAccessor;
		}

		private int ID_BRANCH
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

		private string LOCATION_BRANCH
		{
			get
			{
				return _httpContextAccessor.HttpContext.Session.GetString(LocationBranchSessionKey) ?? null;
			}
			set
			{
				_httpContextAccessor.HttpContext.Session.SetString(LocationBranchSessionKey, value);
			}
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			TempData["Location"] = LOCATION_BRANCH;
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var getAll = await _s_Cart.GetAll(ID_BRANCH, _currentUser);
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

		[HttpPost]
		public IActionResult SubmitCart([FromForm] string cartData)
		{
			return RedirectToAction("InvoiceIndex", "Cart", new { stringifyCarts = cartData });
		}

		// Invoice
		[HttpGet]
		public IActionResult InvoiceIndex(string stringifyCarts)
		{
			TempData["Location"] = LOCATION_BRANCH;
			var invoiceIndex = _s_Cart.InvoiceIndex(stringifyCarts, ID_BRANCH);
			return View(invoiceIndex);
		}

	}
}
