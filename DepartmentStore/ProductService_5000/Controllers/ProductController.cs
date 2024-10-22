using APIGateway.Utilities;
using IdentityServer.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    [Route("/list/[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IS_Product _s_Product;
        private readonly MRes_InfoUser _currentUser;

        public ProductController(IS_Product product,CurrentUserHelper currentUserHelper)
        {
            _s_Product = product;
            _currentUser = currentUserHelper.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var productCategoryToGet = await _s_Product.GetAllProducts();
            return Ok(productCategoryToGet);
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(MReq_Product productRequest)
        {
            try
            {
                var productsToAdd = await _s_Product.AddProductAsync(productRequest, _currentUser);
                return Json(new { result = 1, message = "Sản phẩm đã được thêm thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

    }
}
