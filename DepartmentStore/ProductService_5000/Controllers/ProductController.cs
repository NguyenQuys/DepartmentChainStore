using APIGateway.Response;
using APIGateway.Utilities;
using IdentityServer.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OfficeOpenXml;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Services;
using System.Diagnostics;

namespace ProductService_5000.Controllers
{
    [Route("Product/[action]")]
	public class ProductController : Controller
	{
		private readonly IS_Product _s_Product;
		private readonly MRes_InfoUser _currentUser;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private ISession Session => _httpContextAccessor.HttpContext.Session;

		private const string LocationBranchSessionKey = "LocationBranch";
		private const string IdBranchSessionKey = "IdBranch";

		public ProductController(IS_Product product, CurrentUserHelper currentUserHelper, IHttpContextAccessor httpContextAccessor)
		{
			_s_Product = product;
			_currentUser = currentUserHelper.GetCurrentUser();
			_httpContextAccessor = httpContextAccessor;
		}

        [HttpGet]
        public async Task<IActionResult> Index(int idBranch, string location)
        {
            int? sessionIdBranch = Session.GetInt32(IdBranchSessionKey);
            
			if (sessionIdBranch != null)
            {
                TempData["Location"] = Session.GetString("LocationBranch");
                TempData["IdBranch"] = sessionIdBranch;
            }
            else
            {
                if (idBranch == 0)
                {
                    return Redirect("https://localhost:7076/Branch/ChooseBranchIndex");
                }

                Session.SetString(LocationBranchSessionKey, location);
                Session.SetInt32(IdBranchSessionKey, idBranch);

                TempData["Location"] = location;
                TempData["IdBranch"] = idBranch;
            }

            return View();
        }

        [HttpGet]
		public async Task<IActionResult> GetAllProducts()
		{
			var productsToGet = await _s_Product.GetAllProducts();
			return Json(productsToGet);
		}

		// Excel
		[HttpGet]
		public async Task<IActionResult> GetByName(string productName)
		{
			var productToGet = await _s_Product.GetByName(productName);
			return Json(productToGet);
		}

		[HttpPost]
		public async Task<IActionResult> SearchProduct(string productName)
		{
			var productToGet = await _s_Product.SearchProduct(productName);
			return Json(productToGet);
		}

		[HttpGet]
		public async Task<IActionResult> GetProductsByCategory(int? id)
		{
			try
			{
				var productsToGet = await _s_Product.GetProductsByIdCategory(id, _currentUser);
				return Json(new { result = 1, data = productsToGet });
			}
			catch (Exception ex)
			{
				return Json(new { result = -1, message = ex.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetByIdView(int idProduct)
		{
			TempData["Location"] = Session.GetString(LocationBranchSessionKey);
			TempData["IdBranch"] = Session.GetInt32(IdBranchSessionKey);
			var productToGet = await _s_Product.GetByIdView(idProduct, Session.GetInt32(IdBranchSessionKey) ?? 0);
			return View(productToGet);
		}

		[HttpGet]
		public async Task<IActionResult> GetByIdJson(int idProduct)
		{
			var productToGet = await _s_Product.GetByIdAsync(idProduct);
			return Json(productToGet);
		}

		[Authorize(Roles = "1")]
		[HttpPost]
		public async Task<IActionResult> AddProduct(MReq_Product productRequest)
		{
			try
			{
				var productsToAdd = await _s_Product.AddProductAsync(productRequest, _currentUser);
				return Json(new { result = 1, message = productsToAdd });
			}
			catch (Exception ex)
			{
				return Json(new { result = -1, message = ex.Message });
			}
		}

		[HttpGet, Authorize(Roles = "1")]
		public async Task<IActionResult> ExportSampleProductFileExcel()
		{
			var sampleProductfile = await _s_Product.ExportSampleProductFileExcel();
			var excelFileName = $"Thêm hàng hóa.xlsx";
			return File(sampleProductfile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
		}

		[HttpPost, Authorize(Roles = "1")]
		public async Task<IActionResult> UploadProductByExcel(IFormFile file)
		{
			try
			{
				var result = await _s_Product.UploadProductByExcel(file, _currentUser);
				return Json(new { result = 1, message = result });
			}
			catch (Exception ex)
			{
				return Json(new { result = -1, message = ex.Message });
			}
		}

		[HttpPut]
		[Authorize(Roles = "1")]
		public async Task<IActionResult> UpdateProduct([FromForm] MReq_Product productRequest)
		{
			var productToUpdate = await _s_Product.UpdateProductAsync(productRequest, _currentUser);
			return Json(productToUpdate);
		}

		[HttpPut]
		[Authorize(Roles = "1")]
		public async Task<IActionResult> ChangeStatusProduct(int id)
		{
			var productToChange = await _s_Product.ChangeStatusProduct(id, _currentUser);
			return Json(productToChange);
		}

		[Authorize(Roles = "1")]
		[HttpDelete]
		public async Task<IActionResult> RemoveProduct(int idProduct)
		{
			var productToDelete = await _s_Product.RemoveProduct(idProduct);
			return Json(productToDelete);
		}

		[HttpGet,Authorize]
		public async Task<IActionResult> Product_BranchIndex(int idBranch)
		{
			Session.SetInt32(IdBranchSessionKey, idBranch);
			return View();
		}

		[HttpGet,Authorize]
		public async Task<IActionResult> Product_BranchTab()
		{
			int? sessionIdBranch = Session.GetInt32(IdBranchSessionKey);

			int? idBranch = sessionIdBranch;
			var view = await _s_Product.Product_BranchTab(idBranch);
			return Ok(view);
		}
	}
}