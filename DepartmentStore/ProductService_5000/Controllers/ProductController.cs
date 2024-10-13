using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductService_5000.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;

namespace ProductService_5000.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IActionResult> GetProductsByIdCategory(int id)
        {
            var productCategoryToGet = await _s_Product.GetProductsByIdCategory(id);
            return View(productCategoryToGet);
        }
    }

}
