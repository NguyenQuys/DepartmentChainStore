using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    //[Authorize(Policy = "ProductServicePolicy")]
    public class ProductController : Controller
    {
        private readonly IS_Product _s_Product;
        public ProductController(IS_Product s_Product) 
        {
            _s_Product = s_Product;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var gellAllProducts = await _s_Product.GetAll();
            return View(gellAllProducts);
        }

    }

}
