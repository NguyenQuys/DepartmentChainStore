using IdentityModel.Client;
using IdentityServer.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    public class ProductController : Controller
    {
        private readonly IS_Product _s_Product;
        private readonly MRes_InfoUser _currentUser;

        public ProductController(IS_Product s_Product, CurrentUserHelper currentUserHelper) 
        {
            _s_Product = s_Product;
            _currentUser = currentUserHelper.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var gellAllProducts = await _s_Product.GetAll();
            return View(gellAllProducts);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByIdCategory(int id)
        {
            var productCategoryToGet = await _s_Product.GetProductsByIdCategory(id);
            return View(productCategoryToGet);
        }

        //[Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<IActionResult> ResponseAPIGetProductsByIdCategory(int id)
        {
            var productCategoryToGet = await _s_Product.GetProductsByIdCategory(id);
            return Ok(productCategoryToGet);
        }

        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            if (_currentUser == null)
            {
                return Unauthorized();
            }

            return Ok(_currentUser);
        }
    }

}
